#nullable enable
namespace CozyColdEnvironments.Patterns.Commands
{
    public interface ICommandX
    {
        void Execute();

        void Undo();
    }
}