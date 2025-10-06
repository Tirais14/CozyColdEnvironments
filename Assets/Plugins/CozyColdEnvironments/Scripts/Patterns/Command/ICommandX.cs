#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public interface ICommandX
    {
        void Execute();

        void Undo();
    }
}