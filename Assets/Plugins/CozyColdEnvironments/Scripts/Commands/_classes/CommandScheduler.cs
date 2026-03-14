using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using Cysharp.Threading.Tasks;
using Humanizer;
using R3;
using CCEnvs.Threading.Tasks;
using CCEnvs.Threading;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public sealed class CommandScheduler
        :
        ICommandScheduler,
        IDisposable,
        IFrameRunnerWorkItem
    {
        private readonly static ObjectPool<QueueCommand> queueCmdPool = new(
            Factory.Create(static () => new QueueCommand())
            );

        private readonly ConcurrentQueue<QueueCommand> commands = new();

        private readonly ConcurrentDictionary<CommandSignature, List<QueueCommand>> commandSets = new();

        private readonly ReactiveProperty<bool> isEnabled = new();
        private readonly ReactiveProperty<bool> isRunning = new();

        private readonly CancellationTokenSource _disposeCancellationTokenSource = new();

        private QueueCommand? cmd;

        private bool isRunningFinshingDelayed;

        private int delayFrameCountBeforeRunningFinished;
        private int garbageCmdCount;
        private int garbageCommandCountThreshold = 32;

        private long idleFrameCount;
        private long garbageCollectEveryFrame = 60L;

        private float garbageThreshold = 0.35f;

        private ReactiveCommand<ICommandBase>? scheduleCommandRxCmd;

        public FrameProvider? FrameProvider { get; }

        public bool HasCommands => cmd is not null || commands.IsNotEmpty();
        public bool IsEnabled => isEnabled.Value;
        public bool IsRunning => isRunning.Value;

        public int DelayFrameCountBeforeRunningFinished {
            get => delayFrameCountBeforeRunningFinished;
            set => delayFrameCountBeforeRunningFinished = Math.Clamp(value, 0, int.MaxValue);
        }

        /// <summary>
        /// Checks every 'n' frame the garbage count and if it more than threshold do trigger garbage collect.
        /// <br></br> Set value less than 1 to disable garbage collect.
        /// <br></br>
        /// <br></br> Disabled: The garbage commands will be removed only if it turn has come
        /// </summary>
        public long GarbageCollectEveryFrame {
            get => garbageCollectEveryFrame;
            set => garbageCollectEveryFrame = value;
        }

        /// <summary>
        /// Percentage of garbage commands of all commands count to trigger garbage collect.
        /// <br></br> Set more than 1 to disable garbage collect.
        /// <br></br>
        /// <br></br> Disabled: The garbage commands will be removed only if it turn has come
        /// </summary>
        public float GarbageThreshold {
            get => garbageThreshold;
            set => garbageThreshold = Math.Clamp(value, 0.1f, 1.1f);
        }

        /// <summary>
        /// Scheduled command count to allow garbage collect
        /// </summary>
        public int GargabeCommandCountThreshold {
            get => garbageCommandCountThreshold;
            set => garbageCommandCountThreshold = Math.Clamp(value, 8, int.MaxValue);
        }

        public string Name { get; }

        public object SyncRoot { get; } = new();

        private CancellationToken disposeCancellationToken => _disposeCancellationTokenSource.Token;

        public CommandScheduler(FrameProvider? frameProvider = null, string? name = null)
        {
            frameProvider?.Register(this);

            FrameProvider = frameProvider;
            Name = name ?? string.Empty;

            Enable();
        }

        ~CommandScheduler() => Dispose();

        public static CommandScheduler CreateDefaultRegistered(string? name = null)
        {
            return new CommandScheduler(
                ObservableSystem.DefaultFrameProvider ?? new TimerFrameProvider(1.Milliseconds()),
                name: name
                );
        }

        public void Schedule(ICommandBase cmd)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            CC.Guard.IsNotNull(cmd, nameof(cmd));

            if (!cmd.IsValid)
                throw new ArgumentException($"Comman is not valid. Command: {cmd.Signature}", nameof(cmd));

            if (cmd.IsDone)
            {
                //throw new ArgumentException($"Command: {cmd} invalid status: {cmd.Status}", nameof(cmd));

                this.PrintWarning($"Command is already done. Command: {cmd.Signature}; status {cmd.Status}");
                return;
            }

            ProcessCommandsBy(cmd);

            var queueCmd = queueCmdPool.Get().Value.Set(cmd);

            commands.Enqueue(queueCmd);

            var commandSet = commandSets.GetOrCreateNew(cmd.Signature);

            lock (SyncRoot)
                commandSet.Add(queueCmd);

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Command: {cmd} scheduled");

            scheduleCommandRxCmd?.Execute(cmd);
        }

        [OnInstallExecutable]
        public void Reset()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            OnReset();
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Disposing");

            try
            {
                OnReset();
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                throw;
            }

            _disposeCancellationTokenSource.CancelAndDispose();
            scheduleCommandRxCmd?.Dispose();
            isEnabled.Dispose();
            isRunning.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnFrame()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (!IsEnabled)
                return;

            if (IsIdleFrame())
            {
                OnIdleFrame();
                return;
            }

            StartRunningFrame();

            var loopFuse = LoopFuse.Create();

            while (!CCDisposable.IsDisposed(disposed)
                   &&
                   loopFuse.MoveNext()
                   )
            {
                if (IsCurrentCommandUndone())
                    break;

                if (!TryResolveCommand())
                    break;

                if (!IsCommandReadyToExecute())
                    break;

                ExecuteCommand();
            }

            EndRunningFrame();
        }

        public void Enable()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (isEnabled.Value)
                return;

            isEnabled.Value = true;
        }

        public void Disable()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (!isEnabled.Value)
                return;

            isEnabled.Value = false;
        }

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name})";
        }

        public bool HasCommand(CommandSignature cmdSignature)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (!commandSets.TryGetValue(cmdSignature, out var cmds))
                return false;

            return cmds.Count > 0;
        }

        public bool HasCommand(ICommandBase? command)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (command.IsNull())
                return false;

            return HasCommand(command.Signature);
        }

        public Observable<ICommandBase> ObserveScheduleCommand()
        {
            scheduleCommandRxCmd ??= new ReactiveCommand<ICommandBase>();
            return scheduleCommandRxCmd;
        }

        public Observable<bool> ObserveIsRunningFinsihed()
        {
            return isRunning.Where(static x => !x);
        }

        public Observable<bool> ObserveIsRunningStarted()
        {
            return isRunning.Where(static x => x);
        }

        public Observable<bool> ObserveEnabled()
        {
            return isEnabled.Where(x => x);
        }

        public Observable<bool> ObserveDisabled()
        {
            return isEnabled.Where(x => !x);
        }

        private void OnReset()
        {
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Resetting");

            EraseCurrentCommand();

            lock (SyncRoot)
            {
                int i = 0;
                int cmdCount = commands.Count;

                while (i < cmdCount && commands.TryDequeue(out var cmd))
                    OnCommandDone(cmd);
            }

            commandSets.Clear();

            garbageCmdCount = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ProcessCommandsBy(ICommandBase newCmd)
        {
            if (!newCmd.IsValid)
                return;

            if (commands.IsEmpty())
                return;

            CommandSignature newCmdSignature = newCmd.Signature;

            if (!commandSets.TryGetValue(newCmdSignature, out var equalCmds))
                return;

            QueueCommand cmd;

            for (int i = equalCmds.Count - 1; i >= 0; i--)
            {
                if (!newCmd.IsValid)
                    return;

                lock (SyncRoot)
                    cmd = equalCmds[i];

                if (!IsCommandUndone(cmd.Value))
                {
                    lock (SyncRoot)
                        equalCmds.RemoveAt(i);

                    continue;
                }

                if (CCDebug.Instance.IsEnabled)
                    this.PrintLog($"Command: {cmd} cancelling by added command: {newCmd}.");

                garbageCmdCount++;

                lock (SyncRoot)
                {
                    equalCmds.RemoveAt(i);
                }

                cmd.IsGarbage = true;

                cmd.Value.Cancel();
            }
        }

        private void ClearGarbageCommands()
        {
            using var liveCmds = ListPool<QueueCommand>.Shared.Get();

            if (liveCmds.Value.Capacity < garbageCmdCount)
                liveCmds.Value.Capacity = garbageCmdCount;

            while (commands.TryDequeue(out var cmd))
            {
                if (cmd.IsGarbage || !IsCommandUndone(cmd.Value))
                {
                    OnCommandDone(cmd);
                    continue;
                }

                liveCmds.Value.Add(cmd);
            }

            for (int i = 0; i < liveCmds.Value.Count; i++)
                commands.Enqueue(liveCmds.Value[i]);

            garbageCmdCount = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCommandReadyToExecute()
        {
            if (!IsCurrentCommandUndone())
                return false;

            return cmd!.Value.IsReadyToExecute;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnCommandDone(QueueCommand cmd)
        {
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Command: {cmd} completion");

            if (cmd.Value is null)
            {
                queueCmdPool.Return(cmd.Reset());
                return;
            }

            if (cmd.IsGarbage)
                garbageCmdCount--;

            lock (SyncRoot)
            {
                if (commandSets.TryGetValue(cmd.Value.Signature, out var commandSet))
                    commandSet.Remove(cmd);
            }

            Utilizable.TryUtilizeOrDispose(cmd.Value);

            queueCmdPool.Return(cmd.Reset());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EraseCurrentCommand()
        {
            if (cmd is null)
                return;

            OnCommandDone(cmd);
            cmd = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryResolveCommand()
        {
            EraseCurrentCommand();

            while (commands.TryDequeue(out var cmd))
            {
                if (cmd.IsGarbage || !IsCommandUndone(cmd.Value))
                {
                    OnCommandDone(cmd);
                    continue;
                }

                this.cmd = cmd;

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExecuteCommand()
        {
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Command: {cmd} will be executed");

            switch (cmd!.Value)
            {
                case ICommand cmdSync:
                    {
                        if (cmdSync.ExecuteOnThreadPool)
                        {
#if UNITASK_PLUGIN
                            UniTask.RunOnThreadPool(
                                cmdSync.Execute,
                                cancellationToken: disposeCancellationToken
                                )
                                .Forget();
#else
                            Task.Run(
                                cmdSync.Execute,
                                cancellationToken: disposeCancellationToken
                                );
#endif
                            return;
                        }

                        cmdSync.Execute();
                    }
                    break;
                case ICommandAsync cmdAsync:
                    {
                        if (cmdAsync.ExecuteOnThreadPool)
                        {
#if UNITASK_PLUGIN
                            UniTask.RunOnThreadPool(
                                static async cmdAsync =>
                                {
                                    await ((ICommandAsync)cmdAsync).ExecuteAsync();
                                },
                                cmdAsync,
                                cancellationToken: disposeCancellationToken
                                )
                                .Forget();
#else
                            Task.Run(
                                async () => await cmdAsync.ExecuteAsync(),
                                cancellationToken: disposeCancellationToken
                                );
#endif
                            return;
                        }

                        cmdAsync.ExecuteAsync();
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnRunningFinished()
        {
            isRunningFinshingDelayed = false;
            isRunning.Value = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnIdleFrame()
        {
            idleFrameCount++;

            if (isRunningFinshingDelayed
                &&
                idleFrameCount > DelayFrameCountBeforeRunningFinished)
            {
                OnRunningFinished();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartRunningFrame()
        {
            idleFrameCount = 0;
            isRunning.Value = true;
            isRunningFinshingDelayed = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EndRunningFrame()
        {
            if (commands.IsNotEmpty())
                return;

            if (DelayFrameCountBeforeRunningFinished > 0)
            {
                isRunningFinshingDelayed = true;
                return;
            }

            OnRunningFinished();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsIdleFrame()
        {
            return !HasCommands;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCommandUndone(ICommandBase? cmd)
        {
            return cmd is not null && cmd.IsValid && !cmd!.IsDone;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCurrentCommandUndone()
        {
            return IsCommandUndone(cmd?.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IFrameRunnerWorkItem.MoveNext(long frameCount)
        {
            if (CCDisposable.IsDisposed(disposed))
                return false;

            OnFrame();

            int cmdCount = commands.Count;

            if (garbageCollectEveryFrame > 1L
                &&
                cmdCount >= GargabeCommandCountThreshold)
            {
                if (cmdCount != 0
                    &&
                    frameCount % garbageCollectEveryFrame == 0
                    &&
                    garbageCmdCount / (float)commands.Count >= garbageThreshold
                    )
                {
                    ClearGarbageCommands();
                }
            }

            return true;
        }

        private record QueueCommand
        {
            public ICommandBase Value = null!;
            public bool IsGarbage;

            public QueueCommand Set(ICommandBase value, bool isGarbage = false)
            {
                Value = value;
                IsGarbage = isGarbage;

                return this;
            }

            public QueueCommand Reset()
            {
                Value = null!;
                IsGarbage = true;

                return this;
            }
        }
    }
}
