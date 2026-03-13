#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Threading;
using Cysharp.Threading.Tasks;
using R3;

#pragma warning disable S107
#pragma warning disable S3963
namespace CCEnvs.Patterns.Commands
{
    public abstract partial class CommandBase<TThis> : ICommandBase<TThis>
        where TThis : ICommandBase
    {
        public const string DEFAULT_NAME = "Unnamed";

        protected bool isExecuted;

        protected string name = DEFAULT_NAME;

        private readonly ReactiveProperty<CommandStatus> status = new();

        private List<CancellationTokenSource>? _cancellationTokenSources;
        private List<CancellationTokenRegistration>? _cancellationTokenRegistrations;

        private CancellationTokenRegistration? cancellationTokenRegistration;

        private bool isCancellation;

        private Type? _commandType;

        public string Name {
            get => name;
            set => name = value ?? DEFAULT_NAME;
        }

        public virtual bool IsReadyToExecute => !IsRunning && !IsDone;
        public virtual bool IsCancelled => status.Value == CommandStatus.Canceled;
        public virtual bool IsFaulted => status.Value == CommandStatus.Faulted;
        public virtual bool IsCompleted => status.Value == CommandStatus.Completed;
        public virtual bool IsRunning => isExecuted && !IsDone;
        public bool IsDone => IsCompleted || IsCancelled || IsFaulted;
        public bool IsSingle { get; set; }
        public bool IsResetable { get; }
        public bool IsValid => !CCDisposable.IsDisposed(disposed);
        public bool ExecuteOnThreadPool { get; set; }

        public CommandStatus Status => status.Value;

        public Type CommandType {
            get
            {
                _commandType ??= GetType();

                return _commandType;
            }
        }

        public CommandSignature Signature => new(CommandType, Name);

        protected CancellationToken CancellationToken { get; private set; }

        protected object SyncRoot { get; } = new();

        private List<CancellationTokenSource> cancellationTokenSources {
            get
            {
                _cancellationTokenSources ??= new List<CancellationTokenSource>(1);

                return _cancellationTokenSources;
            }
        }

        private List<CancellationTokenRegistration> cancellationTokenRegistrations {
            get
            {
                _cancellationTokenRegistrations ??= new List<CancellationTokenRegistration>(1);

                return _cancellationTokenRegistrations;
            }
        }

        protected CommandBase(bool isResetable = true)
        {
            IsResetable = isResetable;

            //TrySetDefaultCancellationToken();
        }

        public virtual void Undo()
        {
            ThrowIfDisposed();

            try
            {
                Cancel();
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
            ThrowIfDisposed();

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

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name}; {nameof(Status)}: {Status}; {nameof(IsValid)}: {IsValid})";
        }

        public virtual void Cancel()
        {
            ThrowIfDisposed();

            if (IsDone)
                return;

            if (isCancellation)
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
            ThrowIfDisposed();

            if (!CancellationToken.CanBeCanceled)
            {
                CancellationToken = cancellationToken;
                return this.To<TThis>();
            }

            var linkedTokenSource = CancellationToken.TryLinkTokens(
                cancellationToken,
                out cancellationToken
                );

            if (linkedTokenSource is null)
                return this.To<TThis>();

            lock (SyncRoot)
                cancellationTokenSources.Add(linkedTokenSource);

            CancellationToken = cancellationToken;

            RegisterCancellationTokenToCancel(CancellationToken);

            return this.To<TThis>();
        }

        public async ValueTask WaitForDone(CancellationToken cancellationTokenAdditional = default)
        {
            if (IsDone)
                return;

            using var _ = CancellationToken.LinkTokens(cancellationTokenAdditional, out cancellationTokenAdditional);

            await UniTask.WaitUntil(
                this,
                static @this => @this.IsDone,
                cancellationToken: cancellationTokenAdditional
                );
        }

        public Observable<CommandStatus> ObserveIsDone()
        {
            ThrowIfDisposed();

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

        public Observable<CommandStatus> ObserveStatus()
        {
            return status;
        }

        private int disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
            {
                try
                {
                    OnCancel();
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }

                SetFaulted(null);

                DetachCancellationTokens();

                status.Dispose();
            }
        }

        protected virtual void OnUndo()
        {
        }

        protected virtual void OnReset()
        {
            DetachCancellationTokens();

            isExecuted = false;
            isCancellation = false;

            Name = string.Empty;
            IsSingle = false;
            status.Value = CommandStatus.None;
            ExecuteOnThreadPool = false;
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

                    if (typed.isCancellation
                        ||
                        typed.IsCancelled)
                    {
                        return;
                    }

                    typed.Cancel();
                },
                this
                );

            if (!cancellationTokenRegistration.HasValue)
            {
                cancellationTokenRegistration = reg;
                return;
            }

            lock (SyncRoot)
                cancellationTokenRegistrations.Add(reg);
        }

        //protected void TrySetDefaultCancellationToken()
        //{
        //    DisposeCancellationTokenSources();

        //    var defaultCancellationTokenSource = new CancellationTokenSource();

        //    cancellationTokenSources.Add(defaultCancellationTokenSource);
        //    CancellationToken = defaultCancellationTokenSource.Token;

        //    RegisterCancellationTokenToCancel(CancellationToken);
        //}

        protected void DetachCancellationTokens()
        {
            if (_cancellationTokenRegistrations.IsNotNullOrEmpty())
            {
                lock (SyncRoot)
                {
                    for (int i = cancellationTokenRegistrations.Count - 1; i >= 0; i--)
                    {
                        cancellationTokenRegistrations[i].Dispose();
                        cancellationTokenRegistrations.RemoveAt(i);
                    }

                    if (cancellationTokenRegistrations.Count >= 16)
                        cancellationTokenRegistrations.Capacity = 4;
                }
            }

            if (_cancellationTokenSources.IsNotNullOrEmpty())
            {
                lock (SyncRoot)
                {
                    for (int i = cancellationTokenSources.Count - 1; i >= 0; i--)
                    {
                        cancellationTokenSources[i].Dispose();
                        cancellationTokenSources.RemoveAt(i);
                    }

                    if (cancellationTokenRegistrations.Count >= 16)
                        cancellationTokenSources.Capacity = 4;
                }
            }

            cancellationTokenRegistration?.Dispose();
            cancellationTokenRegistration = default;
            CancellationToken = default;
        }

        //protected void CancelByCancellationToken()
        //{
        //    if (cancellationTokenSources.IsEmpty())
        //        throw new InvalidOperationException($"Command: {this} corrupted and cannot be canceled by token");

        //    cancellationTokenSources[^1].Cancel();
        //    DisposeCancellationTokenSources();
        //}

        protected void ThrowIfDisposed()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
        }

        protected void PrintAlreadyDoneError()
        {
            this.PrintError($"Command is already done and cannot be executed. Command: {Signature}; status {Status}");
        }

        protected void PrintAlreadyExecutedError()
        {
            this.PrintError($"Command is already executed. Command: {Signature}; status {Status}");
        }

        protected void PrintIsNotValidError()
        {
            this.PrintError($"Command is not valid and cannot be executed. Command: {Signature}; status: {Status}");
        }
    }
}
