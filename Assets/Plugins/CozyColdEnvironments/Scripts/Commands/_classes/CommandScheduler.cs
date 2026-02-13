using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using CCEnvs.TypeMatching;
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

        private readonly Dictionary<CommandSignature, HashSet<ICommandBase>> commandSet = new();

        private readonly ReactiveProperty<bool> isEnabled = new();
        private readonly ReactiveProperty<bool> isRunning = new();

        private ICommandBase? cmd;

        private bool cmdExecuted;
        private bool disposed;
        private bool isRunningFinshingDelayed;

        private int delayFrameCountBeforeRunningFinished;
        private int cmdDelayFrameCount;

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

            ValidateDisposed();

            if (!cmd.IsValid)
                throw new ArgumentException($"Command: {cmd} is not valid", nameof(cmd));

            if (cmd.IsDone)
                throw new ArgumentException($"Command: {cmd} invalid status: {cmd.Status}", nameof(cmd));

            ProcessCommandsBy(cmd);

            commands.Enqueue(cmd);

            var cmdSignature = cmd.Signature;

            if (!commandSet.TryGetValue(cmdSignature, out var cmds))
            {
                cmds = new HashSet<ICommandBase>(2);
                commandSet.Add(cmdSignature, cmds);
            }

            cmds.Add(cmd);

            this.PrintLog($"Command: {cmd} scheduled");

            addCommandRxCmd?.Execute(cmd);
        }

        [OnInstallMethod]
        public void Reset()
        {
            this.PrintLog("Resetting");

            cmd?.Dispose();

            EraseCommand();

            commands.UtilizeOrDisposeEach();
            commands.Clear();

            commandSet.Clear();
        }

        public void Dispose()
        {
            if (disposed)
                return;

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

            ValidateDisposed();

            if (IsFrameDelayedByCommand())
            {
                OnCommandDelayedFrame();
                return;
            }

            StartRunningFrame();

            var loopFuse = LoopFuse.Create();

            while (!disposed
                   &&
                   loopFuse.MoveNext())
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
            ValidateDisposed();

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
            if (!commandSet.TryGetValue(commandSignature, out var cmds))
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
            if (commands.IsEmpty())
                return;

            CommandSignature newCmdSignature = newCmd.Signature;

            if (!commandSet.TryGetValue(newCmdSignature, out var equalCmds))
                return;

            foreach (var cmd in equalCmds)
            {
                if (!newCmd.IsValid)
                    return;

                if (!cmd.IsValid)
                    continue;

                if (newCmd.IsSingle
                    &&
                    !cmd.IsDone
                    &&
                    !cmd.IsCancelled)
                {
                    this.PrintLog($"Command: {cmd} cancelling by added command: {newCmd}.");
                    cmd.Undo();
                }
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
            if (!HasUndoneCommand()
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
            if (!HasUndoneCommand())
                return false;

            return cmd!.IsReadyToExecute;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EraseCommand()
        {
            if (cmd is not null)
            {
                this.PrintLog($"Command: {cmd} erasing");

                commandSet[cmd.Signature].Remove(cmd);

                cmdDelayFrameCount = 0;

                cmd.TryUtilizeOrDispose();

                cmd = null;
            }

            cmdExecuted = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryResolveCommand()
        {
            if (HasUndoneCommand())
                return true;

            EraseCommand();

            while (commands.TryDequeue(out cmd))
            {
                if (HasUndoneCommand())
                {
                    cmdDelayFrameCount = cmd.DelayFrameCount;
                    return true;
                }

                commandSet[cmd.Signature].Remove(cmd);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExecuteCommand()
        {
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
        private void ValidateDisposed()
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
        private bool HasUndoneCommand()
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
