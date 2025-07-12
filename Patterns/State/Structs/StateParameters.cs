#nullable enable

using System;
using System.Linq;
using UTIRLib.Patterns.State;

namespace UTIRLib.Patterns.StateMachine
{
    public struct StateParameters
    {
        public UpdateMethod updateAttributes;
        public bool isAbortable;
        public Type[] transitions;

        public StateParameters(UpdateMethod updateAttributes, bool isAbortable, params Type[] transitions)
        {
            this.updateAttributes = updateAttributes;
            this.isAbortable = isAbortable;
            this.transitions = transitions;
        }

        public StateParameters(UpdateMethod updateAttributes, params Type[] transitions) : this(updateAttributes, false, transitions)
        {
        }

        public StateParameters(bool isAbortable, params Type[] transitions) : this(UpdateMethod.Normal, isAbortable, transitions)
        {
        }

        public StateParameters(params Type[] transitions) : this(UpdateMethod.Normal, false, transitions)
        {
        }

        public void SetTransitions(params Type[] stateTypes) => transitions = stateTypes;

        public void SetTransitions(params IReadOnlyState[] states) => SetTransitions(states.Select((object value) => value.GetType()).ToArray());

        public static StateParameters CreateNormal(params Type[] stateTypes) => new(UpdateMethod.Normal, false, stateTypes);

        public static StateParameters CreateAbortableNormal(params Type[] stateTypes) => new(UpdateMethod.Normal, true, stateTypes);

        public static StateParameters CreateFixed(params Type[] stateTypes) => new(UpdateMethod.Fixed, false, stateTypes);

        public static StateParameters CreateAbortableFixed(params Type[] stateTypes) => new(UpdateMethod.Fixed, true, stateTypes);

        public static StateParameters CreateLate(params Type[] stateTypes) => new(UpdateMethod.Late, false, stateTypes);

        public static StateParameters CreateAbortableLate(params Type[] stateTypes) => new(UpdateMethod.Late, true, stateTypes);
    }
}