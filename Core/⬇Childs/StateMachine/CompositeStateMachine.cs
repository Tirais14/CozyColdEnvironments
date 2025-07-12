#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using UTIRLib.Extensions;
using UTIRLib.Patterns.Composite;
using UTIRLib.Patterns.State;

namespace UTIRLib.Patterns.StateMachine
{
    public class CompositeStateMachine : ReadOnlyComposite<IReadOnlyStateMachine>, IReadOnlyStateMachine
    {
        public IReadOnlyState PlayingState {
            get {
                for (int i = 0; i < childsCount; i++)
                {
                    if (!childs[i].IsIdle)
                    {
                        return childs[i].PlayingState;
                    }
                }

                return childs[0].PlayingState;
            }
        }

        public bool IsIdle => childs.All((machine) => machine.IsIdle);
        public bool IsExecuting => childs.Any((machine) => machine.IsExecuting);

        public CompositeStateMachine(IReadOnlyStateMachine[] stateMachines) : base(stateMachines)
        {
        }

        public void Execute()
        {
            for (int i = 0; i < childsCount; i++)
            {
                childs[i].Execute();
            }
        }

        public void SwitchTo(Type stateType)
        {
            for (int i = 0; i < childsCount; i++)
            {
                if (childs[i].Contains(stateType))
                {
                    childs[i].SwitchTo(stateType);
                }
            }
        }

        public void SwitchTo(IReadOnlyState state) => SwitchTo(state.GetType());

        /// <exception cref="KeyNotFoundException"></exception>
        public IReadOnlyState GetState(Type type)
        {
            for (int i = 0; i < childsCount; i++)
            {
                if (childs[i].Contains(type))
                {
                    childs[i].GetState(type);
                }
            }

            throw new KeyNotFoundException(type.Name);
        }

        public TState GetState<TState>() => GetState(typeof(TState)).Convert<TState>()!;

        public bool IsCompleted(Type type) => GetState(type).IsCompleted;

        public bool Contains(Type? type) => childs.Any((machine) => machine.Contains(type));

        public bool Contains(IReadOnlyState? state) => Contains(state?.GetType());

        public bool Contains(StateTransition transition) => childs.Any(machine => machine.Contains(transition));

        public bool IsStateExecuting(Type? type) => childs.Any((machine) => machine.IsStateExecuting(type));

        public bool IsStateExecuting(IReadOnlyState? state) => IsStateExecuting(state?.GetType());
    }
}