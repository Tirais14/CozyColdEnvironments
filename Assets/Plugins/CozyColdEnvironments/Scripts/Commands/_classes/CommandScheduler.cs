using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
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

        private ReactiveCommand<ICommand>? addCommandRxCmd;
        private ReactiveCommand<Unit>? commandsExecutedCmd;

        private Maybe<ICommand> executedCmd;
        private int skipFrameCount;

        public FrameProvider? FrameProvider { get; }
        public bool HasCommands => commands.IsNotEmpty();

        public CommandScheduler()
        {
        }

        public CommandScheduler(FrameProvider frameProvider)
        {
            FrameProvider = frameProvider;
            frameProvider.Register(this);
        }

        public void Schedule(ICommand command)
        {
            CC.Guard.IsNotNull(command, nameof(command));

            if (command.IsCancelled)
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

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            addCommandRxCmd?.Dispose();

            disposed = true;
        }

        public void DoTick()
        {
            if (skipFrameCount > 0)
                skipFrameCount--;

            if (commands.IsEmpty())
                return;

            bool hasCommands = HasCommands;

            while (true)
            {
                if (executedCmd.Map(cmd => cmd.IsRunning).Raw)
                    continue;
                else
                    executedCmd = Maybe<ICommand>.None;

                if (!commands.TryPeek(out ICommand? cmd))
                    break;

                if (!cmd.IsReadyToExecute)
                    break;

                if (cmd.DelayFrameCount > 0)
                {
                    skipFrameCount = cmd.DelayFrameCount - 1;
                    break;
                }

                //Order is important
                cmd = commands.Dequeue();
                cmd.Execute();
                executedCmd = cmd.Maybe();
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
            addCommandRxCmd ??= new ReactiveCommand<ICommand>();
            return addCommandRxCmd;
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
