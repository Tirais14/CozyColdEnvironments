#nullable enable
using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#pragma warning disable S107
#pragma warning disable S3963
namespace CCEnvs.Patterns.Commands
{
    public abstract partial class CommandBase<TThis> : ICommandBase<TThis>
        where TThis : ICommandBase
    {
        protected bool isExecuted;

        protected string name;

        private readonly ReactiveProperty<CommandStatus> status = new();

        private readonly List<CancellationTokenSource> cancellationTokenSources = new(2);
        private readonly List<CancellationTokenRegistration> cancellationTokenRegistrations = new(4);

        private bool isCancellation;

        public string Name {
            get => name;
            set => name = value.IsNullOrWhiteSpace() ?  GetType().ToString() : value;
        }

        public virtual bool IsReadyToExecute => !IsRunning && !IsDone;
        public virtual bool IsCancelled => status.Value == CommandStatus.Canceled;
        public virtual bool IsFaulted => status.Value == CommandStatus.Faulted;
        public virtual bool IsCompleted => status.Value == CommandStatus.Completed;
        public virtual bool IsRunning => isExecuted && !IsDone;
        public bool IsDone => IsCompleted || IsCancelled || IsFaulted;
        public bool IsSingle { get; set; }
        public bool IsResetable { get; }
        public bool IsValid => !disposed;

        public int DelayFrameCount { get; set; }

        public CommandStatus Status => status.Value;

        public Type CommandType { get; }

        public CancellationToken CancellationToken { get; private set; }

        public CommandSignature Signature => new(GetType(), Name);

        protected CommandBase(bool isResetable = true)
        {
            Name = name ?? GetType().ToString();
            IsResetable = isResetable;

            CommandType = GetType();

            SetDefaultCancellationToken();
        }

        public virtual void Undo()
        {
            ValidateDisposed();

            try
            {
                OnCancel();
                OnUndo();
            }
            catch (Exception ex)
            {
                SetFaulted(ex);

                return;
            }

            SetCanceled();
        }

        public bool TryReset()
        {
            ValidateDisposed();

            if (!IsResetable)
                return false;

            Cancel();

            try
            {
                OnReset();
            }
            catch (Exception ex)
            {
                SetFaulted(ex);

                return false;
            }

            SetResetted();

            return true;
        }

        public TThis Reset()
        {
            if (!IsResetable)
                throw new InvalidOperationException($"Command: {this} is not resetable");

            TryReset();

            return this.To<TThis>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandSignature GetCommandSignature()
        {
            return new CommandSignature(GetType(), Name);
        }

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name}; {nameof(Status)}: {Status}; {nameof(IsValid)}: {IsValid})";
        }

        public virtual void Cancel()
        {
            ValidateDisposed();

            if (isCancellation)
                return;

            if (IsDone)
                return;

            isCancellation = true;

            try
            {
                OnCancel();
            }
            catch (Exception ex)
            {
                SetFaulted(ex);

                return;
            }
            finally
            {
                SetDefaultCancellationToken();
                isCancellation = false;
            }

            SetCanceled();
        }

        public IDisposable GetCancellationHandle()
        {
            return Disposable.Create(this,
                static @this =>
                {
                    @this.Cancel();
                });
        }

        public TThis AttachExternalCancellationToken(CancellationToken cancellationToken)
        {
            ValidateDisposed();

            Guard.IsTrue(cancellationToken.CanBeCanceled, nameof(cancellationToken), "Invalid Token");

            var linkedTokenSource = cancellationToken.LinkTokens(CancellationToken);

            cancellationTokenSources.Add(linkedTokenSource);

            CancellationToken = linkedTokenSource.Token;

            RegisterCancellationTokenToCancel(CancellationToken);

            return this.To<TThis>();
        }

        public Observable<CommandStatus> ObserveIsDone()
        {
            ValidateDisposed();

            return status.Where(
                static status =>
                {
                    return status == CommandStatus.Completed
                           ||
                           status == CommandStatus.Canceled
                           ||
                           status == CommandStatus.Faulted;
                });
        }

        private bool disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Cancel();
                SetFaulted(null);
                status.Dispose();
                DisposeCancellationTokenSources();
            }

            disposed = true;
        }

        protected virtual void OnUndo()
        {
        }

        protected virtual void OnReset()
        {
            isExecuted = false;
            isCancellation = false;

            Name = string.Empty;
            DelayFrameCount = 0;
            IsSingle = false;
        }

        protected virtual void OnCancel()
        {

        }

        protected void SetFaulted(Exception? ex)
        {
            status.Value = CommandStatus.Faulted;

            if (ex is not null)
                this.PrintException(ex);
        }

        protected void SetCanceled()
        {
            if (IsFaulted)
                return;

            status.Value = CommandStatus.Canceled;
        }

        protected void SetCompleted()
        {
            status.Value = CommandStatus.Completed;
        }

        protected void SetResetted()
        {
            status.Value = CommandStatus.None;
            isExecuted = false;
        }

        protected void RegisterCancellationTokenToCancel(CancellationToken cancellationToken)
        {
            var reg = cancellationToken.Register(
                static @this =>
                {
                    var typed = @this.To<CommandBase<TThis>>();

                    if (typed.isCancellation)
                        return;

                    typed.Cancel();
                },
                this
                );

            cancellationTokenRegistrations.Add(reg);
        }

        protected void SetDefaultCancellationToken()
        {
            DisposeCancellationTokenSources();

            var defaultCancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSources.Add(defaultCancellationTokenSource);
            CancellationToken = defaultCancellationTokenSource.Token;

            RegisterCancellationTokenToCancel(CancellationToken);
        }

        protected void DisposeCancellationTokenSources()
        {
            cancellationTokenRegistrations.DisposeEach();
            cancellationTokenRegistrations.Clear();

            cancellationTokenSources.DisposeEach();
            cancellationTokenSources.Clear();
        }

        protected void CancelByCancellationToken()
        {
            if (cancellationTokenSources.IsEmpty())
                throw new InvalidOperationException($"Command: {this} corrupted and cannot be canceled by token");

            cancellationTokenSources[^1].Cancel();
            DisposeCancellationTokenSources();
        }

        protected void ValidateDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().ToString());
        }
    }
}
