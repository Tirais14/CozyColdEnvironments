#nullable enable
using CommunityToolkit.Diagnostics;
using System;

namespace CCEnvs.Patterns.Commands
{
    public partial class AnonymousCommand : Command, ICommand
    {
        private readonly Func<bool>? isReadyToExecute;
        private readonly Action execute;
        private readonly Action? onReset;

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (isReadyToExecute?.Invoke() ?? true);
            }
        }

        public AnonymousCommand(
            Action execute,
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
            Guard.IsNotNull(execute);

            this.isReadyToExecute = isReadyToExecute;
            this.execute = execute;
            this.onReset = onReset;
        }

        public override string ToString()
        {
            return $"{nameof(CommandName)}: {CommandName}";
        }

        protected override void OnExecute() => execute();

        protected override void OnReset()
        {
            base.OnReset();
            onReset?.Invoke();
        }
    }
    public class AnonymousCommand<T> : Command, ICommand
    {
        private readonly T state;
        private readonly Action<T> execute;
        private readonly Func<T, bool>? isReadyToExecute;
        private readonly Action<T>? onReset;

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (isReadyToExecute?.Invoke(state) ?? true);
            }
        }

        public AnonymousCommand(T state,
            Action<T> execute,
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
            Guard.IsNotNull(isReadyToExecute);
            Guard.IsNotNull(execute);

            this.state = state;
            this.isReadyToExecute = isReadyToExecute;
            this.execute = execute;
            this.onReset = onReset;
        }

        public override string ToString()
        {
            return $"{nameof(CommandName)}: {CommandName}; {nameof(IsDone)}: {IsDone}";
        }

        protected override void OnExecute() => execute(state);

        protected override void OnReset()
        {
            base.OnReset();
            onReset?.Invoke(state);
        }
    }
}
