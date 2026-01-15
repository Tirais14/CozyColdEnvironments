using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using Newtonsoft.Json.Converters;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public sealed class CommandScheduler : ICommandScheduler, IDisposable, IFrameRunnerWorkItem
    {
        private readonly Queue<ICommand> commands = new();
        private readonly HashSet<CommandInfo> addedCommandInfos = new();

        private ReactiveCommand<ICommand>? addCommandRxCmd;
        private ReactiveCommand<Unit>? commandsExecutedCmd;

        private ICommand? cmd;
        private bool cmdExecuted;
        private int idleFrameCount;

        public FrameProvider? FrameProvider { get; }
        public bool HasCommands => commands.IsNotEmpty();
        public bool IsEnabled { get; private set; }

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
            if (!IsEnabled)
                return;

            if (IsIdleFrame())
                return;

            if (commands.IsEmpty())
                return;

            bool hasCommands = HasCommands;
            int iterationCount = 0;

            while (true)
            {
                if (iterationCount > 10000)
                    throw new InvalidOperationException("Time out");

                iterationCount++;

                ResolveCommand();

                if (IsIdleFrame())
                    break;

                if (!IsCommandReadyToExecute(cmd))
                    break;

                ExecuteCommand(cmd!);
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

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable()
        {
            IsEnabled = false;
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
            DoTick();
            return true;
        }
    }
}
