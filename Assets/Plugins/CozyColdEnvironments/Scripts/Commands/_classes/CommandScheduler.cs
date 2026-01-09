using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
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
        private ReactiveCommand<Returnables.Unit>? commandsExecutedCmd;

        public bool HasCommands => commands.IsNotEmpty();

        public void AddCommand(ICommand command)
        {
            CC.Guard.IsNotNull(command, nameof(command));

            if (command.IsCancelled)
                return;

            commands.Enqueue(command);

            ValidateCommandsByAdded(command);

            addedCommandInfos.Add(command.GetCommandInfo());

            addCommandCmd?.Execute(command);
        }

        public void Clear()
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
            bool hasCommands = HasCommands;

            while (commands.TryPeek(out ICommand cmd))
            {
                if (cmd.IsCancelled)
                    continue;

                if (!cmd.IsReadyToExecute)
                    break;

                cmd = commands.Dequeue();
                cmd.Execute();
            }

            if (commandsExecutedCmd is not null
                &&
                hasCommands
                &&
                commands.IsEmpty())
            {
                commandsExecutedCmd.Execute(Returnables.Unit.Default);
            }
        }

        public Observable<ICommand> ObserveAddCommand()
        {
            addCommandCmd ??= new ReactiveCommand<ICommand>();
            return addCommandCmd;
        }

        public Observable<Returnables.Unit> ObserveCommandsExecuted()
        {
            commandsExecutedCmd ??= new ReactiveCommand<Returnables.Unit>();
            return commandsExecutedCmd;
        }

        private void ValidateCommandsByAdded(ICommand newCmd)
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
