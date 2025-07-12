#nullable enable

using UTIRLib.Patterns.State;

namespace UTIRLib.Patterns.StateMachine
{
    public abstract class MonoStateMachine : MonoReadOnlyStateMachine, IStateMachine
    {
        public void AddState(IReadOnlyState state) => stateMachineInternal.AddState(state);

        public void AddStates(params IReadOnlyState[] states) => stateMachineInternal.AddStates(states);

        public void AddTransition(StateTransition transition) => stateMachineInternal.AddTransition(transition);

        public void AddTransitions(params StateTransition[] transitions) =>
            stateMachineInternal.AddTransitions(transitions);

        public void RemoveState(IReadOnlyState? state) => stateMachineInternal.RemoveState(state);

        public void RemoveStates(params IReadOnlyState?[] states) => stateMachineInternal.RemoveStates(states);

        public void RemoveTransition(StateTransition transition) => stateMachineInternal.RemoveTransition(transition);

        public void RemoveTransitions(params StateTransition[] transitions) =>
            stateMachineInternal.RemoveTransitions(transitions);
    }
}