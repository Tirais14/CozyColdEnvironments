#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public interface ICommand
    {
        bool IsReadyToExecute { get; }
        bool IsCancelled { get; }
        string Name { get; }

        void Execute();

        void Undo();
    }
}