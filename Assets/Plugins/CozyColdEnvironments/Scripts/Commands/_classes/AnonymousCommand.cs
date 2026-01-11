#nullable enable
using CommunityToolkit.Diagnostics;
using System;

namespace CCEnvs.Patterns.Commands
{
    public partial class AnonymousCommand : Command, ICommand
    {
        private readonly Func<bool>? isReadyToExecute;
        private readonly Action execute;

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (isReadyToExecute?.Invoke() ?? true);
            }
        }

        public AnonymousCommand(
            Action execute,
            Func<bool>? isReadyToExecute = null,
            string? name = null,
            bool isSingle = false)
            :
            base(name: name,
                 isSingle: isSingle)
        {
            Guard.IsNotNull(execute);

            this.isReadyToExecute = isReadyToExecute;
            this.execute = execute;
        }

        public override string ToString()
        {
            return $"{nameof(CommandName)}: {CommandName}";
        }

        protected override void OnExecute() => execute();
    }
    public class AnonymousCommand<T> : Command, ICommand
    {
        private readonly T state;
        private readonly Predicate<T>? isReadyToExecute;
        private readonly Action<T> execute;

        public override bool IsReadyToExecute {
            get
            {
                return base.IsReadyToExecute && (isReadyToExecute?.Invoke(state) ?? true);
            }
        }

        public AnonymousCommand(T state,
            Action<T> execute,
            Predicate<T>? isReadyToExecute = null,
            string? name = null,
            bool singleCommand = false)
            :
            base(name: name,
                 isSingle: singleCommand)
        {
            Guard.IsNotNull(isReadyToExecute);
            Guard.IsNotNull(execute);

            this.state = state;
            this.isReadyToExecute = isReadyToExecute;
            this.execute = execute;
        }

        public override string ToString()
        {
            return $"{nameof(CommandName)}: {CommandName}; {nameof(state)}: {state}";
        }

        protected override void OnExecute() => execute(state);
    }
}
