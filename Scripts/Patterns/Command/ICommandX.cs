#nullable enable
namespace UTIRLib.Patterns.Commands
{
    public interface ICommandX
    {
        void Execute();

        void Undo();
    }
}