using UTIRLib.Patterns.Commands;

#nullable enable
namespace UTIRLib.Tickables
{
    public abstract class ATickerCommand<T> 
        : 
        ICommandX

        where T : ITickableBase
    {
        public T Tickable { get; }

        protected ATickerCommand(T tickable)
        {
            Tickable = tickable;
        }

        public abstract void Execute();

        public void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}
