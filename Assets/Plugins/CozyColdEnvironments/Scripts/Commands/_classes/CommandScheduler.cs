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
        private readonly Queue<ICommand> commands = new();
        private readonly HashSet<CommandInfo> addedCommandInfos = new();
        private readonly ReactiveProperty<bool> isEnabled = new();
        private readonly ReactiveProperty<bool> isRunning = new();

        private ICommand? cmd;

        private bool cmdExecuted;
        private bool disposed;

        private int delayFrameCountBeforeRunningFinished;
        private int delayCommandFrameCount;

        private ulong idleFrameCount;

        private ReactiveCommand<ICommand>? addCommandRxCmd;

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

        public void Schedule(ICommand command)
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

            if (commands.IsEmpty())
            {
                idleFrameCount++;
                return;
            }

            ValidateDisposed();

            if (IsFrameDelayedByCommand())
                return;

            idleFrameCount = 0;

            if (!isRunning.Value)
                isRunning.Value = true;

            int iterationCount = 0;

            while (true)
            {
                if (iterationCount > 100000)
                    throw new InvalidOperationException("Time out");

                iterationCount++;

                ResolveCommand();

                if (IsFrameDelayedByCommand())
                    break;

                if (!IsCommandReadyToExecute(cmd))
                    break;

                ExecuteCommand(cmd!);
            }

            if (commands.IsEmpty())
                isRunning.Value = false;
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

        public Observable<ICommand> ObserveAddCommand()
        {
            addCommandRxCmd ??= new ReactiveCommand<ICommand>();
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

        private void ProcessCommandsBy(ICommand newCmd)
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

        private bool IsCommandReseted(ICommand cmd)
        {
            return cmdExecuted && !cmd.IsDone && !cmd.IsRunning;
        }

        private bool IsCommandReadyToExecute(ICommand? cmd)
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

        private void ExecuteCommand(ICommand cmd)
        {
            cmd.ExecuteAsync();
            cmdExecuted = true;
        }

        private void ValidateDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().ToString());
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
