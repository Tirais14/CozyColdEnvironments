#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public sealed class AnonymousCommandAsync : PoolableCommandAsync
    {
        public Func<bool>? ExecutePredicate { get; set; }
        public Func<CancellationToken, ValueTask>? ExecuteAction { get; set; }
        public Action? ResetAction { get; set; }
        public Action? CancelAction { get; set; }

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (ExecutePredicate?.Invoke() ?? true);
            }
        }

        public AnonymousCommandAsync(
            string? name = null,
            bool isSingle = false,
            int delayFrameCount = 0)
            :
            base(name: name,
                 isSingle: isSingle,
                 delayFrameCount: delayFrameCount)
        {
        }

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name}; {nameof(Status)}: {Status})";
        }

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            if (ExecuteAction is null)
                return;

            var task = ExecuteAction(cancellationToken);

            if (!task.IsCompleted)
                await task;
        }

        protected override void OnReset()
        {
            base.OnReset();

            try
            {
                ResetAction?.Invoke();
            }
            finally
            {
                ExecuteAction = null;
                ExecutePredicate = null;
                ResetAction = null;
                CancelAction = null;
            }
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            CancelAction?.Invoke();
        }
    }
    public sealed class AnonymousCommandAsync<T> : PoolableCommandAsync
    {
        public T State { get; set; }
        public Func<T, CancellationToken, ValueTask>? ExecuteAction { get; set; }
        public Func<T, bool>? ExecutePredicate { get; set; }
        public Action<T>? ResetAction { get; set; }
        public Action<T>? CancelAction { get; set; }

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (ExecutePredicate?.Invoke(State) ?? true);
            }
        }

        public AnonymousCommandAsync(
            string? name = null,
            bool isSingle = false,
            int delayFrameCount = 0)
            :
            base(name: name,
                 isSingle: isSingle,
                 delayFrameCount: delayFrameCount)
        {
        }

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name}; {nameof(Status)}: {Status}; {nameof(State)}: {State})";
        }

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            if (ExecuteAction is null)
                return;

            var task = ExecuteAction(State, cancellationToken);

            if (!task.IsCompleted)
                await task;
        }

        protected override void OnReset()
        {
            base.OnReset();

            try
            {
                ResetAction?.Invoke(State);
            }
            finally
            {
                State = default;
                ExecuteAction = null;
                ExecutePredicate = null;
                ResetAction = null;
                CancelAction = null;
            }
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            CancelAction?.Invoke(State);
        }
    }
}
