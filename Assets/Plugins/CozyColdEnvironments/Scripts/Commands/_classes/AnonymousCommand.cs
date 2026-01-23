#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public sealed class AnonymousCommand : Command
    {
        private readonly Action? onExecute;
        private readonly Func<bool>? isReadyToExecute;
        private readonly Action? onReset;

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
            string? name = null,
            bool isSingle = false,
            bool isResetable = false,
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

    }

    public sealed class AnonymousCommand<T> : Command
    {
        private readonly T state;
        private readonly Action<T>? onExecute;
        private readonly Func<T, bool>? isReadyToExecute;
        private readonly Action<T>? onReset;

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
            string? name = null,
            bool isSingle = false,
            bool isResetable = false,
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
    }
}
