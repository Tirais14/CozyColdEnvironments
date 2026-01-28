#nullable enable
using CommunityToolkit.Diagnostics;
using System;

namespace CCEnvs.Patterns.Commands
{
    public sealed class AnonymousCommand : PoolableCommand
    {
        public Action? ExecuteAction { get; set; }
        public Func<bool>? ExecutePredicate { get; set; }
        public Action? ResetAction { get; set; }
        public Action? CancelAction { get; set; }

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (ExecutePredicate?.Invoke() ?? true);
            }
        }

        public AnonymousCommand(
            string? name = null,
            bool isSingle = false,
            int delayFrameCount = 0)
            :
            base(name: name,
                 isSingle: isSingle,
                 delayFrameCount: delayFrameCount)
        {
        }

        protected override void OnExecute()
        {
            if (ExecuteAction is null)
                return;

            ExecuteAction();
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

    public sealed class AnonymousCommand<T> : Command
    {
        public T State { get; set; }
        public Action<T>? ExecuteAction { get; set; }
        public Func<T, bool>? ExecutePredicate { get; set; }
        public Action<T>? ResetAction { get; set; }
        public Action<T>? CancelAction { get; set; }

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (ExecutePredicate?.Invoke(State) ?? true);
            }
        }

        public AnonymousCommand(
            string? name = null,
            bool isSingle = false,
            int delayFrameCount = 0)
            :
            base(name: name,
                 isSingle: isSingle,
                 delayFrameCount: delayFrameCount)
        {
        }

        protected override void OnExecute()
        {
            if (ExecuteAction is null)
                return;

            Guard.IsNotNull(State, nameof(State));

            ExecuteAction(State);
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
