#nullable enable
using System;
using System.Runtime.CompilerServices;

namespace CCEnvs.Patterns.Commands
{
    public partial class Command 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnonymousCommand Create(
            Action execute,
            Func<bool>? isReadyToExecute = null,
            string? name = null,
            bool singleCommand = false)
        {
            return new AnonymousCommand(
                execute,
                isReadyToExecute,
                name: name,
                singleCommand: singleCommand
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnonymousCommand<T> Create<T>(
            T state,
            Action<T> execute,
            Predicate<T>? isReadyToExecute = null,
            string? name = null,
            bool singleCommand = false)
        {
            return new AnonymousCommand<T>(
                state,
                execute,
                isReadyToExecute,
                name: name,
                singleCommand: singleCommand
                );
        }
    }

    public abstract partial class Command : ICommand
    {
        private bool executed;
        private bool isFaulted;
        private bool isCanceled;

        public static ICommand Completed { get; } = new CompletedCommand();

        public string CommandName { get; } = string.Empty;

        public virtual bool IsReadyToExecute => !IsDone;
        public virtual bool IsCancelled => isCanceled;
        public virtual bool IsFaulted => isFaulted;
        public virtual bool IsCompleted { get; protected set; }
        public virtual bool IsRunning => executed && IsDone;

        public bool IsDone => !executed && (IsCompleted || IsCancelled || IsFaulted);
        public bool IsSingle { get; }

        protected Command(
            bool isSingle,
            string? name = null)
        {
            CommandName = name ?? GetType().ToString();
            IsSingle = isSingle;
        }

        public void Execute()
        {
            if (IsRunning || IsDone)
                return;

            executed = true;
            try
            {
                OnExecute();
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                isFaulted = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Undo()
        {
            if (IsDone)
                return;

            OnUndo();
            isCanceled = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandInfo GetCommandInfo()
        {
            return new CommandInfo(GetType(), CommandName);
        }

        public override string ToString()
        {
            if (CommandName.IsNullOrWhiteSpace())
                return GetType().ToString();
            else
                return CommandName;
        }

        protected abstract void OnExecute();

        protected virtual void OnUndo()
        {
        }
    }
}
