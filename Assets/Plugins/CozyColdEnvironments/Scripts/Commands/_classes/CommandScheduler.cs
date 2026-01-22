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
        private readonly Queue<ICommandAsync> commands = new();

        private readonly HashSet<CommandInfo> addedCommandInfos = new();

        private readonly ReactiveProperty<bool> isEnabled = new();
        private readonly ReactiveProperty<bool> isRunning = new();

        private ICommandAsync? cmd;

        private bool cmdExecuted;
        private bool disposed;
        private bool isRunningFinshingDelayed;

        private int delayFrameCountBeforeRunningFinished;
        private int delayCommandFrameCount;

        private ulong idleFrameCount;

        private ReactiveCommand<ICommandAsync>? addCommandRxCmd;

        public FrameProvider? FrameProvider { get; }

        public bool HasCommands => commands.IsNotEmpty();
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

        public void Schedule(ICommandAsync command)
        {
            CC.Guard.IsNotNull(command, nameof(command));
            ValidateDisposed();

            if (command.IsDone)
                return;

            commands.Enqueue(command);

            ProcessCommandsBy(command);

            addedCommandInfos.Add(command.GetCommandInfo());

            addCommandRxCmd?.Execute(command);
        }

        public void Reset()
        {
            commands.Clear();
        }

        public void Dispose()
        {
            if (disposed)
                return;

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
                DoIdleFrame();
                return;
            }

            ValidateDisposed();

            if (IsFrameDelayedByCommand())
                return;

            StartRunningFrame();

#if CC_DEBUG
            var loopFuse = LoopFuse.Create();
#endif

            while (!disposed
#if CC_DEBUG
                &&
                loopFuse.MoveNext()
#endif
                )
            {
                ResolveCommand();

                if (IsFrameDelayedByCommand())
                    break;

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

        public Observable<ICommandAsync> ObserveAddCommand()
        {
            addCommandRxCmd ??= new ReactiveCommand<ICommandAsync>();
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

        private void ProcessCommandsBy(ICommandAsync newCmd)
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

        private bool IsFrameDelayedByCommand()
        {
            if (cmd is null
                ||
                cmd.IsDone
                ||
                delayCommandFrameCount < 1)
            {
                return false;
            }
            
            return delayCommandFrameCount-- < 1;
        }

        private bool IsCommandReseted(ICommandAsync cmd)
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
            if (cmd.IsNotNull() && cmd.IsRunning && !IsCommandReseted(cmd))
                return;

            EraseCommand();

            if (!commands.TryDequeue(out cmd))
                return;

            delayCommandFrameCount = cmd.DelayFrameCount;
        }

        private void ExecuteCommand()
        {
            cmd!.ExecuteAsync();
            cmdExecuted = true;
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

        private void DoIdleFrame()
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
            return commands.IsEmpty();
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
