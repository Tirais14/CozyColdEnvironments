#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public interface ICommand : ICommandBase
    {
        void Execute();

        ICommand Reset();
    }
}
