#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public sealed class AnonymousCommandAsync : PoolableCommandAsync
    {
        private readonly Func<bool>? isReadyToExecute;
        private readonly Func<CancellationToken, ValueTask>? onExecute;
        private readonly Action? onReset;
        private readonly Action? onCancel;

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (isReadyToExecute?.Invoke() ?? true);
            }
        }

        public AnonymousCommandAsync(
            Func<CancellationToken, ValueTask>? onExecute,
            Func<bool>? isReadyToExecute = null,
            Action? onReset = null!,
            Action? onCancel = null!,
            string? name = null,
            bool isSingle = false,
            int delayFrameCount = 0)
            :
            base(name: name,
                 isSingle: isSingle,
                 delayFrameCount: delayFrameCount)
        {
            this.isReadyToExecute = isReadyToExecute;
            this.onExecute = onExecute;
            this.onReset = onReset;
            this.onCancel = onCancel;
        }

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name}; {nameof(Status)}: {Status})";
        }

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            if (onExecute is null)
                return;

            var task = onExecute(cancellationToken);

            if (!task.IsCompleted)
                await task;
        }

        protected override void OnReset()
        {
            base.OnReset();
            onReset?.Invoke();
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            onCancel?.Invoke();
        }
    }
    public sealed class AnonymousCommandAsync<T> : PoolableCommandAsync
    {
        private readonly T state;
        private readonly Func<T, CancellationToken, ValueTask>? onExecute;
        private readonly Func<T, bool>? isReadyToExecute;
        private readonly Action<T>? onReset;
        private readonly Action<T>? onCancel;

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (isReadyToExecute?.Invoke(state) ?? true);
            }
        }

        public AnonymousCommandAsync(T state,
            Func<T, CancellationToken, ValueTask>? onExecute,
            Func<T, bool>? isReadyToExecute = null,
            Action<T>? onReset = null,
            Action<T>? onCancel = null,
            string? name = null,
            bool isSingle = false,
            int delayFrameCount = 0)
            :
            base(name: name,
                 isSingle: isSingle,
                 delayFrameCount: delayFrameCount)
        {
            this.state = state;
            this.isReadyToExecute = isReadyToExecute;
            this.onExecute = onExecute;
            this.onReset = onReset;
            this.onCancel = onCancel;
        }

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name}; {nameof(Status)}: {Status}; {nameof(state)}: {state})";
        }

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            if (onExecute is null)
                return;

            var task = onExecute(state, cancellationToken);

            if (!task.IsCompleted)
                await task;
        }

        protected override void OnReset()
        {
            base.OnReset();
            onReset?.Invoke(state);
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            onCancel?.Invoke(state);
        }
    }
}
