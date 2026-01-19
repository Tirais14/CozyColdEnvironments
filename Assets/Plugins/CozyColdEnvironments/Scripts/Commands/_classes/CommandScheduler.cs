using CCEnvs.Collections;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public sealed class CommandScheduler 
        : 
        ICommandScheduler, 
        IDisposable, 
        IFrameRunnerWorkItem,
        ISwitchable
    {
        private readonly Queue<ICommand> commands = new();
        private readonly HashSet<CommandInfo> addedCommandInfos = new();
        private readonly ReactiveProperty<bool> isEnabled = new();

        private ReactiveCommand<ICommand>? addCommandRxCmd;
        private ReactiveCommand<Unit>? commandsExecutedCmd;

        private ICommand? cmd;
        private bool cmdExecuted;
        private int idleFrameCount;
        private bool disposed;

        public FrameProvider? FrameProvider { get; }
        public bool HasCommands => commands.IsNotEmpty();
        public bool IsEnabled => isEnabled.Value;   

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

            if (command.IsDone)
                return;

            commands.Enqueue(command);

            ProcessCommandsBy(command);

            addedCommandInfos.Add(command.GetCommandInfo());

            addCommandRxCmd?.Execute(command);

            Enable();
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
            commandsExecutedCmd?.Dispose();

            disposed = true;
        }

        public void DoTick()
        {
            if (!IsEnabled)
                return;

            if (disposed)
                throw new ObjectDisposedException(GetType().ToString());

            if (IsIdleFrame())
                return;

            bool hasCommands = HasCommands;
            int iterationCount = 0;

            while (true)
            {
                if (iterationCount > 100000)
                    throw new InvalidOperationException("Time out");

                iterationCount++;

                ResolveCommand();

                if (IsIdleFrame())
                    break;

                if (!IsCommandReadyToExecute(cmd))
                    break;

                ExecuteCommand(cmd!);
            }

            if (hasCommands
                &&
                commands.IsEmpty())
            {
                commandsExecutedCmd?.Execute(Unit.Default);
                Disable();
            }
        }

        public void Enable()
        {
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

        public Observable<Unit> ObserveCommandsExecuted()
        {
            commandsExecutedCmd ??= new ReactiveCommand<Unit>();
            return commandsExecutedCmd;
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

        private bool IsIdleFrame()
        {
            if (idleFrameCount > 0)
                idleFrameCount--;

            return idleFrameCount > 0;
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
            commands.TryDequeue(out cmd);
        }

        private void ExecuteCommand(ICommand cmd)
        {
            cmd.ExecuteAsync();
            cmdExecuted = true;
        }

        bool IFrameRunnerWorkItem.MoveNext(long frameCount)
        {
            if (disposed)
                return false;

            DoTick();

            return true;
        }
    }
}
