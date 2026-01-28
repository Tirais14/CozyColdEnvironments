#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public interface ICommand : ICommandBase<ICommand>
    {
        void Execute();
    }
}
