#nullable enable
using System;

namespace CCEnvs.Patterns.Commands
{
    public sealed class AnonymousCommand : PoolableCommand
    {
        private readonly Action? onExecute;
        private readonly Func<bool>? isReadyToExecute;
        private readonly Action? onReset;
        private readonly Action? onCancel;

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (isReadyToExecute?.Invoke() ?? true);
            }
        }

        public AnonymousCommand(
            Action? onExecute,
            Func<bool>? isReadyToExecute = null,
            Action? onReset = null!,
            Action? onCancel = null,
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

        protected override void OnExecute()
        {
            if (onExecute is null)
                return;

            onExecute();
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

    public sealed class AnonymousCommand<T> : Command
    {
        private readonly T state;
        private readonly Action<T>? onExecute;
        private readonly Func<T, bool>? isReadyToExecute;
        private readonly Action<T>? onReset;
        private readonly Action<T>? onCancel;

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (isReadyToExecute?.Invoke(state) ?? true);
            }
        }

        public AnonymousCommand(T state,
            Action<T>? onExecute,
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

        protected override void OnExecute()
        {
            if (onExecute is null)
                return;

            onExecute(state);
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
