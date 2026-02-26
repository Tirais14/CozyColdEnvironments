using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Pools;
using Humanizer;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public sealed class CommandScheduler 
        : 
        ICommandScheduler, 
        IDisposable, 
        IFrameRunnerWorkItem
    {
        private readonly Queue<ICommandBase> commands = new();

        private readonly Dictionary<CommandSignature, HashSet<ICommandBase>> commandSets = new();

        private readonly HashSet<ICommandBase> garbageCommands = new(16);

        private readonly ReactiveProperty<bool> isEnabled = new();
        private readonly ReactiveProperty<bool> isRunning = new();

        private ICommandBase? cmd;

        private bool cmdExecuted;
        private bool disposed;
        private bool isRunningFinshingDelayed;

        private int delayFrameCountBeforeRunningFinished;
        private int cmdDelayFrameCount;
        private int garbageCmdCount;

        private long idleFrameCount;

        private ReactiveCommand<ICommandBase>? addCommandRxCmd;

        public FrameProvider? FrameProvider { get; }

        public bool HasCommands => cmd is not null || commands.IsNotEmpty();
        public bool IsEnabled => isEnabled.Value;
        public bool IsRunning => isRunning.Value && idleFrameCount >= DelayFrameCountBeforeRunningFinished;

        public int DelayFrameCountBeforeRunningFinished {
            get => delayFrameCountBeforeRunningFinished;
            set => delayFrameCountBeforeRunningFinished = Math.Clamp(value, 0, int.MaxValue);
        }

        public string Name { get; }

        public object SyncRoot { get; } = new();

        public CommandScheduler(FrameProvider? frameProvider = null, string? name = null)
        {
            frameProvider?.Register(this);

            FrameProvider = frameProvider;
            Name = name ?? string.Empty;

            Enable();
        }

        public static CommandScheduler CreateDefaultRegistered()
        {
            return new CommandScheduler(
                ObservableSystem.DefaultFrameProvider ?? new TimerFrameProvider(1.Milliseconds()),
                name: nameof(CC)
                );
        }

        public void Schedule(ICommandBase cmd)
        {
            CC.Guard.IsNotNull(cmd, nameof(cmd));

            ThrowIfDisposed();

            if (!cmd.IsValid)
                throw new ArgumentException($"Command: {cmd} is not valid", nameof(cmd));

            if (cmd.IsDone)
                throw new ArgumentException($"Command: {cmd} invalid status: {cmd.Status}", nameof(cmd));

            ProcessCommandsBy(cmd);

            HashSet<ICommandBase> commandSet;

            lock (SyncRoot)
            {
                commands.Enqueue(cmd);
                commandSet = commandSets.GetOrCreateNew(cmd.Signature);
            }

            commandSet.Add(cmd);

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Command: {cmd} scheduled");

            addCommandRxCmd?.Execute(cmd);
        }

        [OnInstallExecutable]
        public void Reset()
        {
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Resetting");

            cmd?.Dispose();

            EraseCurrentCommand();

            commands.UtilizeOrDisposeEach(bufferized: false);
            commands.Clear();
            commands.TrimExcess();

            commandSets.Clear();
            commandSets.TrimExcess();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Disposing");

            Reset();

            addCommandRxCmd?.Dispose();
            isEnabled.Dispose();
            isRunning.Dispose();

            disposed = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnFrame()
        {
            if (!IsEnabled)
                return;

            if (IsIdleFrame())
            {
                OnIdleFrame();
                return;
            }

            ThrowIfDisposed();

            if (IsFrameDelayedByCommand())
            {
                OnCommandDelayedFrame();
                return;
            }

            StartRunningFrame();

#if CC_DEBUG_ENABLED
            var loopFuse = LoopFuse.Create();
#endif

            while (!disposed
#if CC_DEBUG_ENABLED
                   &&
                   loopFuse.MoveNext()
#endif
                   )
            {
                if (!TryResolveCommand())
                    break;

                if (IsFrameDelayedByCommand())
                {
                    OnCommandDelayedFrame();
                    break;
                }

                if (!IsCommandReadyToExecute())
                    break;

                ExecuteCommand();
            }

            EndRunningFrame();
        }

        public void Enable()
        {
            ThrowIfDisposed();

            if (isEnabled.Value)
                return;

            isEnabled.Value = true;
        }

        public void Disable()
        {
            if (!isEnabled.Value)
                return;

            isEnabled.Value = false;
        }

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name})";
        }

        public bool HasCommand(CommandSignature commandSignature)
        {
            if (!commandSets.TryGetValue(commandSignature, out var cmds))
                return false;

            return cmds.Count > 0;
        }

        public bool HasCommand(ICommandBase? command)
        {
            if (command.IsNull())
                return false;

            return HasCommand(command.Signature);
        }

        public Observable<ICommandBase> ObserveAddCommand()
        {
            addCommandRxCmd ??= new ReactiveCommand<ICommandBase>();
            return addCommandRxCmd;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ProcessCommandsBy(ICommandBase newCmd)
        {
            if (!newCmd.IsValid)
                return;

            if (commands.IsEmpty())
                return;

            CommandSignature newCmdSignature = newCmd.Signature;

            HashSet<ICommandBase> equalCmds;

            lock (SyncRoot)
            {
                if (!commandSets.TryGetValue(newCmdSignature, out equalCmds))
                    return;
            }

            foreach (var cmd in equalCmds)
            {
                if (!newCmd.IsValid)
                    return;

                if (!cmd.IsValid)
                    continue;

                if (newCmd.IsSingle
                    &&
                    !cmd.IsDone)
                {
                    if (CCDebug.Instance.IsEnabled)
                        this.PrintLog($"Command: {cmd} cancelling by added command: {newCmd}.");

                    garbageCommands.Add(cmd);

                    cmd.Cancel();
                }
            }

            if (garbageCommands.Count > 32)
                ClearGarbageCommands();
        }

        private void ClearGarbageCommands()
        {
            lock (SyncRoot)
            {
                using var filteredCmds = ListPool<ICommandBase>.Shared.Get();

                if (filteredCmds.Value.Capacity < garbageCmdCount)
                    filteredCmds.Value.Capacity = garbageCmdCount;

                foreach (var cmd in commands)
                {
                    if (cmd.IsValid && !cmd.IsDone)
                    {
                        filteredCmds.Value.Add(cmd);
                        OnCommandDone(cmd);
                    }
                }

                commands.Clear();

                for (int i = 0; i < filteredCmds.Value.Count; i++)
                    commands.Enqueue(filteredCmds.Value[i]);

                garbageCmdCount = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnCommandDelayedFrame()
        {
            cmdDelayFrameCount--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsFrameDelayedByCommand()
        {
            if (!IsCurrentCommandUndone()
                ||
                cmdDelayFrameCount < 1)
            {
                return false;
            }
            
            return cmdDelayFrameCount > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCommandReseted(ICommandBase cmd)
        {
            return cmdExecuted && !cmd.IsDone && !cmd.IsRunning;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCommandReadyToExecute()
        {
            if (!IsCurrentCommandUndone())
                return false;

            return cmd!.IsReadyToExecute;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnCommandDone(ICommandBase cmd)
        {
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Command: {cmd} completing");

            var commandSet = commandSets[cmd.Signature];

            commandSet.Remove(cmd);
            garbageCommands.Remove(cmd);

            if (commandSet.IsEmpty())
                commandSets.Remove(cmd.Signature);

            cmd.TryUtilizeOrDispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EraseCurrentCommand()
        {
            if (cmd is not null)
            {
                OnCommandDone(cmd);
                cmd = null;
            }

            cmdDelayFrameCount = 0;
            cmdExecuted = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryResolveCommand()
        {
            if (IsCurrentCommandUndone())
                return true;

            EraseCurrentCommand();

            while (commands.TryDequeue(out cmd))
            {
                if (IsCurrentCommandUndone())
                {
                    cmdDelayFrameCount = cmd.DelayFrameCount;
                    return true;
                }

                OnCommandDone(cmd);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExecuteCommand()
        {
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Command: {cmd} will be executed");

            switch (cmd)
            {
                case ICommand cmdSync:
                    cmdSync.Execute();
                    break;
                case ICommandAsync cmdAsync:
                    cmdAsync.ExecuteAsync();
                    break;
                default:
                    throw new InvalidOperationException();
            }

            cmdExecuted = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().ToString());
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EndRunningFrame()
        {
            if (commands.IsEmpty())
            {
                if (DelayFrameCountBeforeRunningFinished > 0)
                {
                    isRunningFinshingDelayed = true;
                    return;
                }

                OnRunningFinished();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsIdleFrame()
        {
            return !HasCommands;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCurrentCommandUndone()
        {
            return cmd is not null && cmd.IsValid && !cmd!.IsDone && !IsCommandReseted(cmd!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IFrameRunnerWorkItem.MoveNext(long frameCount)
        {
            if (disposed)
                return false;

            OnFrame();

            return true;
        }
    }
}
