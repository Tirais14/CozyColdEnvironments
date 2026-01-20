#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public sealed class AnonymousCommand : Command, ICommand
    {
        private readonly Func<bool>? isReadyToExecute;
        private readonly Func<CancellationToken, ValueTask>? onExecute;
        private readonly Action? onReset;

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (isReadyToExecute?.Invoke() ?? true);
            }
        }

        public AnonymousCommand(
            Func<CancellationToken, ValueTask>? onExecute,
            Func<bool>? isReadyToExecute = null,
            Action? onReset = null!,
            string? name = null,
            bool isSingle = false,
            bool isResetable = true,
            int delayFrameCount = 0)
            :
            base(name: name,
                 isSingle: isSingle,
                 isResetable: isResetable,
                 delayFrameCount: delayFrameCount)
        {
            this.isReadyToExecute = isReadyToExecute;
            this.onExecute = onExecute;
            this.onReset = onReset;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}";
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

    }
    public sealed class AnonymousCommand<T> : Command, ICommand
    {
        private readonly T state;
        private readonly Func<T, CancellationToken, ValueTask>? onExecute;
        private readonly Func<T, bool>? isReadyToExecute;
        private readonly Action<T>? onReset;

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (isReadyToExecute?.Invoke(state) ?? true);
            }
        }

        public AnonymousCommand(T state,
            Func<T, CancellationToken, ValueTask>? onExecute,
            Func<T, bool>? isReadyToExecute = null,
            Action<T>? onReset = null,
            string? name = null,
            bool isSingle = false,
            bool isResetable = true,
            int delayFrameCount = 0)
            :
            base(name: name,
                 isSingle: isSingle,
                 isResetable: isResetable,
                 delayFrameCount: delayFrameCount)
        {
            this.state = state;
            this.isReadyToExecute = isReadyToExecute;
            this.onExecute = onExecute;
            this.onReset = onReset;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}; {nameof(IsDone)}: {IsDone}";
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
    }
}
