//#nullable enable
//using CCEnvs.Collections;
//using CCEnvs.Patterns.Commands;
//using Cysharp.Threading.Tasks;
//using R3;
//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Threading;

//namespace CCEnvs.Unity.Commands
//{
//    public class CommandScheduler : IDisposable, ICommandScheduler
//    {
//        private readonly Queue<ICommand> commands = new();

//        private readonly HashSet<ICommand> commandSet = new(
//            new AnonymousEqualityComparer<ICommand>(
//                static (left, right) => left.CommandName == right.CommandName && left.GetType() == right.GetType(),
//                static x => HashCode.Combine(x.CommandName, x.GetType())
//                ));
        
//        private ReactiveCommand<ICommand>? addCommand;
//        private ReactiveCommand<Returnables.Unit>? commandsExecuted;
//        private CancellationTokenSource? onDisableCancellationTokenSource;

//        public bool HasCommands => commands.IsNotEmpty();
//        public bool IsRunning { get; private set; }

//        public void Schedule(ICommand command)
//        {
//            CC.Guard.IsNotNull(command, nameof(command));

//            if (command.IsCancelled)
//                return;

//            OnCommandAdd(command);
//            if (command.IsCancelled)
//                return;

//            commands.Enqueue(command);
//            commandSet.Add(command);
//            addCommand?.Execute(command);
//        }

//        public void Reset()
//        {
//            commands.Clear();
//            commandSet.Clear();
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public void DoTick()
//        {
//            while (commands.TryPeek(out ICommand command) && command.IsReadyToExecute)
//            {
//                if (command.IsCancelled)
//                {
//                    var x = commands.Dequeue();
//                    commandSet.Remove(x);
//                    this.PrintLog($"Command: {x} cancelled.");
//                }
//                else
//                {
//                    var x = commands.Dequeue();
//                    commandSet.Remove(x);

//                    try
//                    {
//                        x.Execute();
//                        this.PrintLog($"Command: {x} executed.");
//                    }
//                    catch (Exception ex)
//                    {
//                        x.PrintException(ex);
//                    }
//                }
//            }

//            if (commands.IsEmpty())
//                commandsExecuted?.Execute(Returnables.Unit.Default);
//        }

//        public Observable<ICommand> ObserveAddCommand()
//        {
//            addCommand ??= new ReactiveCommand<ICommand>();
//            return addCommand;
//        }

//        public Observable<Returnables.Unit> ObserveCommandsExecuted()
//        {
//            commandsExecuted ??= new ReactiveCommand<Returnables.Unit>();
//            return commandsExecuted;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="frameStartTiming"></param>
//        /// <returns>Disposable which on Dispose invokes <see cref="Stop"/></returns>
//        public IDisposable Start(
//            PlayerLoopTiming frameStartTiming = PlayerLoopTiming.LastInitialization,
//            CancellationToken cancellationToken = default)
//        {
//            if (IsRunning)
//                return Disposable.Empty;

//            onDisableCancellationTokenSource = new CancellationTokenSource();

//            IsRunning = true;
//            var task = UniTask.Create((@this: this, frameStartTiming, cancellationToken: onDisableCancellationTokenSource.Token),
//                static async input =>
//                {
//                    while (input.@this.IsRunning)
//                    {
//                        await UniTask.WaitForEndOfFrame(cancellationToken: input.cancellationToken);
//                        input.@this.DoTick();

//                        await UniTask.NextFrame(
//                            timing: input.frameStartTiming,
//                            cancellationToken: input.cancellationToken
//                            );
//                    }
//                });

//            if (cancellationToken != CancellationToken.None)
//                task.AttachExternalCancellation(cancellationToken);

//            IsRunning = true;
//            return Disposable.Create(this, @this => @this.Stop());
//        } 

//        public void Stop()
//        {
//            if (!IsRunning)
//                return;

//            if (onDisableCancellationTokenSource is not null)
//            {
//                onDisableCancellationTokenSource.Cancel();
//                onDisableCancellationTokenSource.Dispose();
//                onDisableCancellationTokenSource = null;
//            }
//        }

//        private bool disposed;
//        public void Dispose() => Dispose(true);
//        protected virtual void Dispose(bool disposing)
//        {
//            if (disposed)
//                return;

//            if (disposing)
//            {
//                Stop();
//                addCommand?.Dispose();
//                commandsExecuted?.Dispose();
//                onDisableCancellationTokenSource?.Dispose();
//            }

//            disposed = true;
//        }

//        protected virtual void OnCommandAdd(ICommand command)
//        {
//            bool isSingleCommand = command.IsSingle;
//            bool hasUndoCommands = command.UndoCommandsOnAdd.Length > 0;
//            if (commandSet.Contains(command)
//                && 
//                (isSingleCommand
//                ||
//                hasUndoCommands
//                ))
//            {
//                CommandInfo info = command.GetCommandInfo();
//                foreach (var cmd in commands)
//                {
//                    if (isSingleCommand && info.IsMatch(cmd))
//                    {
//                        cmd.Undo();
//                        continue;
//                    }

//                    if (hasUndoCommands)
//                    {
//                        var undoCommandsOnAdd = command.UndoCommandsOnAdd;
//                        int count = undoCommandsOnAdd.Length;
//                        for (int i = 0; i < count; i++)
//                        {
//                            if (undoCommandsOnAdd[i].IsMatch(cmd))
//                                cmd.Undo();
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
