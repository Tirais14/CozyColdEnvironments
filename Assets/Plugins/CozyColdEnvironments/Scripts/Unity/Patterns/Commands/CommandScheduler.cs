#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Commands;
using CCEnvs.Returnables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UniRx;

namespace CCEnvs.Unity.Commands
{
    public class CommandScheduler : IDisposable, ICommandScheduler
    {
        private readonly Queue<ICommand> queue = new();
        private ReactiveCommand<ICommand>? addCommand;
        private ReactiveCommand<Mock>? commandsExecuted;
        private CancellationTokenSource? commandsExecutedCancellationTokenSource;

        public CancellationToken CommandsExecutedCancellationToken {
            get
            {
                commandsExecutedCancellationTokenSource ??= new CancellationTokenSource();
                return commandsExecutedCancellationTokenSource.Token;
            }
        }

        public bool HasCommands => queue.IsNotEmpty();

        public void AddCommand(ICommand command)
        {
            CC.Guard.IsNotNull(command, nameof(command));

            queue.Enqueue(command);
            addCommand?.Execute(command);
        }

        public void Clear() => queue.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DoTick()
        {
            while (queue.TryPeek(out ICommand command) && command.IsReadyToExecute)
            {
                if (command.IsCancelled)
                {
                    var x = queue.Dequeue();
                    CCDebug.PrintLog($"Command {x} Cancelled.");
                }
                else
                {
                    var x = queue.Dequeue();

                    try
                    {
                        x.Execute();
                        CCDebug.PrintLog($"Command {x} Executed.");
                    }
                    catch (Exception ex)
                    {
                        x.PrintException(ex);
                    }
                }
            }

            if (queue.IsEmpty())
            {
                commandsExecuted?.Execute(Mock.Default);

                if (commandsExecutedCancellationTokenSource is not null)
                {
                    commandsExecutedCancellationTokenSource?.Cancel();
                    commandsExecutedCancellationTokenSource?.Dispose();
                    commandsExecutedCancellationTokenSource = null;
                }
            }
        }

        public IObservable<ICommand> ObserveAddCommand()
        {
            addCommand ??= new ReactiveCommand<ICommand>();
            return addCommand;
        }

        public IObservable<Mock> ObserveCommandsExecuted()
        {
            commandsExecuted ??= new ReactiveCommand<Mock>();
            return commandsExecuted;
        }

        private bool disposed;
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                addCommand?.Dispose();
                commandsExecuted?.Dispose();
            }

            disposed = true;
        }
    }
}
