#nullable enable

namespace UTIRLib.Animation
{
    public interface IAnimatorStateMachine
    {
        public bool IsStateExecuting(string? stateName, int layerIndex);

        public bool IsStateExecuting(IAnimatorState animatorState);
    }

    public interface IAnimatorStateMachine<in T> : IAnimatorStateMachine
    {
        public bool IsStateExecuting(T animatorState);
    }
}