using CCEnvs.Collections;
using R3;
using Serilog;
using System;
using System.Collections.Generic;

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

        private readonly HashSet<CommandInfo> addedCommandInfos = new();

        private readonly ReactiveProperty<bool> isEnabled = new();
        private readonly ReactiveProperty<bool> isRunning = new();

        private ICommandBase? cmd;

        private bool cmdExecuted;
        private bool disposed;
        private bool isRunningFinshingDelayed;

        private int delayFrameCountBeforeRunningFinished;
        private int cmdDelayFrameCount;

        private ulong idleFrameCount;

        private ReactiveCommand<ICommandBase>? addCommandRxCmd;

        public FrameProvider? FrameProvider { get; }

        public bool HasCommands => cmd is not null || commands.IsNotEmpty();
        public bool IsEnabled => isEnabled.Value;
        public bool IsRunning => isRunning.Value && idleFrameCount >= (ulong)DelayFrameCountBeforeRunningFinished;

        public int DelayFrameCountBeforeRunningFinished {
            get => delayFrameCountBeforeRunningFinished;
            set => delayFrameCountBeforeRunningFinished = Math.Clamp(value, 0, int.MaxValue);
        }

        public CommandScheduler()
        {
            Enable();
        }

        public CommandScheduler(FrameProvider frameProvider)
            :
            this()
        {
            FrameProvider = frameProvider;
            frameProvider.Register(this);
        }

        public void Schedule(ICommandBase command)
        {
            CC.Guard.IsNotNull(command, nameof(command));
            ValidateDisposed();

            if (command.IsDone)
                return;

            ProcessCommandsBy(command);

            commands.Enqueue(command);

            addedCommandInfos.Add(command.GetCommandInfo());

            addCommandRxCmd?.Execute(command);
        }

        public void Reset()
        {
            EraseCommand();

            cmd?.Dispose();
            commands.DisposeEach();
            commands.Clear();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            Reset();
            addCommandRxCmd?.Dispose();
            isEnabled.Dispose();
            isRunning.Dispose();

            disposed = true;
        }

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
                ResolveCommand();

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

        private void ProcessCommandsBy(ICommandBase newCmd)
        {
            if (commands.IsEmpty())
                return;

            CommandInfo newCmdInfo = newCmd.GetCommandInfo();

            if (!addedCommandInfos.Contains(newCmdInfo))
                return;

            foreach (var cmd in commands)
            {
                if (newCmd.IsSingle && cmd.GetCommandInfo() == newCmdInfo)
                    cmd.Undo();
            }
        }

        private void OnCommandDelayedFrame()
        {
            cmdDelayFrameCount--;
        }

        private bool IsFrameDelayedByCommand()
        {
            if (cmd is null
                ||
                cmd.IsDone
                ||
                cmdDelayFrameCount < 1)
            {
                return false;
            }
            
            return cmdDelayFrameCount > 0;
        }

        private bool IsCommandReseted(ICommandBase cmd)
        {
            return cmdExecuted && !cmd.IsDone && !cmd.IsRunning;
        }

        private bool IsCommandReadyToExecute()
        {
            if (cmd.IsNull()
                ||
                cmd.IsDone)
            {
                return false;
            }

            return cmd.IsReadyToExecute;
        }

        private void EraseCommand()
        {
            if (cmd.IsNotNull())
                addedCommandInfos.Remove(cmd.GetCommandInfo());

            cmd = null;
            cmdExecuted = false;
        }

        private void ResolveCommand()
        {
            if (cmd is not null && !cmd.IsDone && !IsCommandReseted(cmd))
                return;

            EraseCommand();

            if (!commands.TryDequeue(out cmd))
                return;

            cmdDelayFrameCount = cmd.DelayFrameCount;
        }

        private void ExecuteCommand()
        {
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

            this.PrintLog($"Command: {cmd} executed");
        }

        private void ValidateDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().ToString());
        }

        private void OnRunningFinished()
        {
            isRunningFinshingDelayed = false;
            isRunning.Value = false;
        }

        private void OnIdleFrame()
        {
            idleFrameCount++;

            if (isRunningFinshingDelayed
                &&
                idleFrameCount > (ulong)DelayFrameCountBeforeRunningFinished)
            {
                OnRunningFinished();
            }
        }

        private void StartRunningFrame()
        {
            idleFrameCount = 0;
            isRunning.Value = true;
        }

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

        private bool IsIdleFrame()
        {
            return !HasCommands;
        }

        bool IFrameRunnerWorkItem.MoveNext(long frameCount)
        {
            if (disposed)
                return false;

            OnFrame();

            return true;
        }
    }
}
