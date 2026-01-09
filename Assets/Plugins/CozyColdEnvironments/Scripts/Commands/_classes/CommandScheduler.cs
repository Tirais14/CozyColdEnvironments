using CCEnvs.Collections;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public sealed class CommandScheduler : ICommandScheduler, IDisposable, IFrameRunnerWorkItem
    {
        private readonly Queue<ICommand> commands = new();
        private readonly HashSet<CommandInfo> addedCommandInfos = new();

        private ReactiveCommand<ICommand>? addCommandCmd;
        private ReactiveCommand<Unit>? commandsExecutedCmd;

        public bool HasCommands => commands.IsNotEmpty();

        public void Schedule(ICommand command)
        {
            CC.Guard.IsNotNull(command, nameof(command));

            if (command.IsCancelled)
                return;

            commands.Enqueue(command);

            ProcessCommandsBy(command);

            addedCommandInfos.Add(command.GetCommandInfo());

            addCommandCmd?.Execute(command);
        }

        public void Reset()
        {
            commands.Clear();
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            addCommandCmd?.Dispose();

            disposed = true;
        }

        public void DoTick()
        {
            if (commands.IsEmpty())
                return;

            bool hasCommands = HasCommands;

            while (commands.TryPeek(out ICommand cmd))
            {
                if (cmd.IsDone)
                {
                    _ = commands.Dequeue();
                    continue;
                }

                if (cmd.IsRunning)
                    break;

                if (!cmd.IsReadyToExecute)
                    break;

                cmd.Execute();
            }

            if (commandsExecutedCmd is not null
                &&
                hasCommands
                &&
                commands.IsEmpty())
            {
                commandsExecutedCmd.Execute(Unit.Default);
            }
        }

        public Observable<ICommand> ObserveAddCommand()
        {
            addCommandCmd ??= new ReactiveCommand<ICommand>();
            return addCommandCmd;
        }

        public Observable<Unit> ObserveCommandsExecuted()
        {
            commandsExecutedCmd ??= new ReactiveCommand<Unit>();
            return commandsExecutedCmd;
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

        bool IFrameRunnerWorkItem.MoveNext(long frameCount)
        {
            DoTick();
            return true;
        }
    }
}
