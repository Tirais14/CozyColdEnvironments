#nullable enable
using CCEnvs.Patterns.Commands;
using CCEnvs.Returnables;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UniRx;

namespace CCEnvs.Unity.Commands
{
    public class CommandScheduler : IDisposable, ICommandScheduler
    {
        private readonly Queue<ICommand> commands = new();

        private readonly HashSet<ICommand> commandSet = new(
            new AnonymousEqualityComparer<ICommand>(
                static (left, right) => left.CommandName == right.CommandName && left.GetType() == right.GetType(),
                static x => HashCode.Combine(x.CommandName, x.GetType())
                ));
        
        private ReactiveCommand<ICommand>? addCommand;
        private ReactiveCommand<Mock>? commandsExecuted;
        private CancellationTokenSource? onDisableCancellationTokenSource;

        public bool HasCommands => commands.IsNotEmpty();
        public bool IsEnabled { get; private set; }

        public void AddCommand(ICommand command)
        {
            CC.Guard.IsNotNull(command, nameof(command));

            if (command.IsCancelled)
                return;

            OnCommandAdd(command);
            if (command.IsCancelled)
                return;

            commands.Enqueue(command);
            commandSet.Add(command);
            addCommand?.Execute(command);
        }

        public void Clear()
        {
            commands.Clear();
            commandSet.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DoTick()
        {
            while (commands.TryPeek(out ICommand command) && command.IsReadyToExecute)
            {
                if (command.IsCancelled)
                {
                    var x = commands.Dequeue();
                    commandSet.Remove(x);
                    this.PrintLog($"Command: {x} cancelled.");
                }
                else
                {
                    var x = commands.Dequeue();
                    commandSet.Remove(x);

                    try
                    {
                        x.Execute();
                        this.PrintLog($"Command: {x} executed.");
                    }
                    catch (Exception ex)
                    {
                        x.PrintException(ex);
                    }
                }
            }

            if (commands.IsEmpty())
                commandsExecuted?.Execute(Mock.Default);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameStartTiming"></param>
        /// <returns>Disposable which on Dispose invokes <see cref="Stop"/></returns>
        public IDisposable Start(PlayerLoopTiming frameStartTiming = PlayerLoopTiming.LastInitialization)
        {
            if (IsEnabled)
                return Disposable.Empty;

            onDisableCancellationTokenSource = new CancellationTokenSource();

            UniTask.Create((@this: this, frameStartTiming, cancellationToken: onDisableCancellationTokenSource.Token),
                static async input =>
                {
                    while (input.@this.IsEnabled)
                    {
                        await UniTask.WaitForEndOfFrame(cancellationToken: input.cancellationToken);
                        input.@this.DoTick();

                        await UniTask.NextFrame(
                            input.frameStartTiming,
                            cancellationToken: input.cancellationToken
                            );
                    }
                })
                .Forget();

            IsEnabled = true;
            return Disposable.CreateWithState(this, @this => @this.Stop());
        } 

        public void Stop()
        {
            if (!IsEnabled)
                return;

            if (onDisableCancellationTokenSource is not null)
            {
                onDisableCancellationTokenSource.Cancel();
                onDisableCancellationTokenSource.Dispose();
                onDisableCancellationTokenSource = null;
            }
        }

        private bool disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Stop();
                addCommand?.Dispose();
                commandsExecuted?.Dispose();
                onDisableCancellationTokenSource?.Dispose();
            }

            disposed = true;
        }

        protected void OnCommandAdd(ICommand command)
        {
            if (command.IsSingle
                &&
                commandSet.Contains(command))
            {
                CommandInfo info = command.GetCommandInfo();
                foreach (var cmd in commands)
                {
                    if (!info.IsMatch(cmd))
                        continue;

                    cmd.Undo();
                }
            }

            if (command.UndoCommandsOnAdd.IsNotEmpty())
            {
                CommandInfo info;
                for (int i = 0; i < command.UndoCommandsOnAdd.Length; i++)
                {
                    info = command.UndoCommandsOnAdd[i];
                    foreach (var cmd in commands)
                    {
                        if (info.IsMatch(cmd))
                            cmd.Undo();
                    }
                }
            }
        }
    }
}
