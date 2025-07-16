#nullable enable
namespace UTIRLib.Patterns.States
{
    public interface IFixedState : IStateBase, IExecutable
    {
        void FixedExecute();
    }
}
