#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

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

                return builder.Reset();
            }
        }

        static Command()
        {
            builder = new AnonymousCommandBuilder();
            builder.OnBuilded += _ => isBuilderBusy = false;
        }
    }

    public abstract partial class Command : ICommand, IDisposable, IEquatable<Command>
    {
        public static ICommand Completed { get; } = new CompletedCommand();

        private bool isExecuted;
        private bool isCanceled;
        private bool isFaulted;
        private bool isCompleted;

        private CancellationTokenSource? cancellationTokenSource;
        private readonly Type type;

        public string CommandName { get; } = string.Empty;

        public virtual bool IsReadyToExecute => !IsRunning && !IsDone;
        public virtual bool IsCancelled => isCanceled;
        public virtual bool IsFaulted => isFaulted;
        public virtual bool IsCompleted => isCompleted;
        public virtual bool IsRunning => isExecuted && !IsDone;

        public bool IsDone => IsCompleted || IsCancelled || IsFaulted;
        public bool IsSingle { get; }
        public bool IsResetable { get; }

        public int DelayFrameCount { get; set; }

        protected Command(
            bool isSingle = false,
            string? name = null,
            bool isResetable = true,
            int delayFrameCount = 0)
        {
            CommandName = name ?? GetType().ToString();
            IsSingle = isSingle;
            IsResetable = isResetable;
            DelayFrameCount = delayFrameCount;

            type = GetType();
        }

        public static bool operator ==(Command? left, Command right)
        {
            return ReferenceEquals(left, right)
                   ||
                   (left is not null && left.Equals(right));
        }

        public static bool operator !=(Command? left, Command right)
        {
            return !(left == right);
        }

        public async ValueTask ExecuteAsync()
        {
            if (IsRunning || IsDone)
                return;

            isExecuted = true;

            CancelAndDisposeCancellationTokenSource();
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await OnExecuteAsync(cancellationTokenSource.Token);
                isCompleted = true;
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case TaskCanceledException:
                        isCanceled = true;
                        break;
                    default:
                        isFaulted = true;
                        this.PrintException(ex);
                        break;
                }
            }

            if (!IsDone
                &&
                (cancellationTokenSource is null
                ||
                cancellationTokenSource.IsCancellationRequested))
            {
                isCanceled = true;
            }

            CancelAndDisposeCancellationTokenSource();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Undo()
        {
            if (IsDone)
                return;

            if (cancellationTokenSource is null)
                throw new InvalidOperationException($"{nameof(CancellationTokenSource)} not found");

            CancelAndDisposeCancellationTokenSource();

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
            if (!IsResetable)
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

        bool disposed;
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                CancelAndDisposeCancellationTokenSource();

            disposed = true;
        }

        protected abstract ValueTask OnExecuteAsync(CancellationToken cancellationToken);

        protected virtual void OnUndo()
        {
        }

        protected virtual void OnReset()
        {
        }

        private void CancelAndDisposeCancellationTokenSource()
        {
            if (cancellationTokenSource is not null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        public bool Equals(Command other)
        {
            return CommandName == other.CommandName
                   &&
                   type == other.type;
        }

        public override bool Equals(object obj)
        {
            return obj is Command typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(type, CommandName);
        }
    }
}
