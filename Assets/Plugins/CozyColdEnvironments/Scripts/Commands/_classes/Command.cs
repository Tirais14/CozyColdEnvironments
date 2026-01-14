#nullable enable
using System;
using System.Runtime.CompilerServices;

#pragma warning disable S107
#pragma warning disable S3963
namespace CCEnvs.Patterns.Commands
{
    public partial class Command 
    {
        private static AnonymousCommandBuilder builder;
        private static bool isBuilderBusy;

        public static AnonymousCommandBuilder Builder {
            get
            {
                if (isBuilderBusy)
                    return new AnonymousCommandBuilder();

                return builder;
            }
        }

        static Command()
        {
            builder = new AnonymousCommandBuilder();
            builder.OnBuilded += _ => isBuilderBusy = false;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static AnonymousCommand Create(
        //    Action execute,
        //    Func<bool>? isReadyToExecute = null,
        //    Action? onReset = null,
        //    string? name = null,
        //    bool isSingle = false,
        //    bool isResetable = true,
        //    int delayFrameCount = 0)
        //{
        //    return new AnonymousCommand(
        //        execute,
        //        isReadyToExecute,
        //        onReset: onReset,
        //        name: name,
        //        isSingle: isSingle,
        //        isResetable: isResetable,
        //        delayFrameCount: delayFrameCount
        //        );
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static AnonymousCommand<T> Create<T>(
        //    T state,
        //    Action<T> execute,
        //    Predicate<T>? isReadyToExecute = null,
        //    Action<T>? onReset = null,
        //    string? name = null,
        //    bool isSingle = false, 
        //    bool isResetable = true,
        //    int delayFrameCount = 0)
        //{
        //    return new AnonymousCommand<T>(
        //        state,
        //        execute,
        //        isReadyToExecute,
        //        onReset: onReset,
        //        name: name,
        //        isSingle: isSingle,
        //        isResetable: isResetable,
        //        delayFrameCount: delayFrameCount
        //        );
        //}
    }

    public abstract partial class Command : ICommand
    {
        public static ICommand Completed { get; } = new CompletedCommand();

        private bool isExecuted;
        private bool isCanceled;
        private bool isFaulted;
        private bool isCompleted;

        public string CommandName { get; } = string.Empty;

        public virtual bool IsReadyToExecute => !IsDone;
        public virtual bool IsCancelled => isCanceled;
        public virtual bool IsFaulted => isFaulted;
        public virtual bool IsCompleted => isCompleted;
        public virtual bool IsRunning => isExecuted && !IsDone;

        public bool IsDone => IsCompleted || IsCancelled || IsFaulted;
        public bool IsSingle { get; }
        public bool IsResetable { get; }

        public int DelayFrameCount { get; }

        protected Command(
            bool isSingle,
            string? name = null,
            bool isResetable = true,
            int delayFrameCount = 0)
        {
            CommandName = name ?? GetType().ToString();
            IsSingle = isSingle;
            IsResetable = isResetable;
            DelayFrameCount = delayFrameCount;
        }

        public void Execute()
        {
            if (IsRunning || IsDone)
                return;

            isExecuted = true;
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

        public bool TryReset()
        {
            if (!IsResetable)
                return false;

            if (!IsDone && !IsRunning)
                return true;

            if (IsRunning)
                Undo();

            isExecuted = false;
            isCanceled = false;
            isFaulted = false;
            isCompleted = false;

            OnReset();

            return true;
        }

        public ICommand Reset()
        {
            if (IsResetable)
                throw new InvalidOperationException($"Command: {this} is not resetable");

            TryReset();

            return this;
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

        protected virtual void OnReset()
        {
        }
    }
}
