#nullable enable
namespace UTIRLib.Patterns.States
{
    /// <summary>
    /// Realize this to set switch conditions
    /// </summary>
    public interface IStateMachineSwitchStrategy<T>
        where T : IStateBase
    {
        T DefaultState { get; }

        T GetNextState();
    }
}
