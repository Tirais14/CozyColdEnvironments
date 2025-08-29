namespace CCEnvs.Patterns.Commands
{
#nullable enable

    public interface ICommandReciever
    {
        void AddCommand(ICommandX command);
    }

    public interface ICommandReciever<in T>
        where T : ICommandX
    {
        void AddCommand(T command);
    }
}