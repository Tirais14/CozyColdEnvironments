#nullable enable
using System.Runtime.CompilerServices;

namespace CCEnvs.Patterns.Commands
{
    public abstract class ACommand : ICommand
    {
        public abstract bool IsReadyToExecute { get; }
        public bool IsCancelled { get; private set; }
        public string Name { get; }

        public ACommand()
        {
            Name = GetType().ToString();
        }

        public ACommand(string? name)
        {
            Name = name ?? GetType().ToString();
        }

        public abstract void Execute();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Undo() => IsCancelled = true;

        public override string ToString() => Name;
    }
}
