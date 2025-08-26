using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UTIRLib.Diagnostics;
using UTIRLib.Patterns.Factory;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.Patterns.States
{
    public static class StateMachineHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CollectionArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void InjectStates(IStateMachine stateMachine,
                                        FieldInfo[] stateFields,
                                        IState[] states)
        {
            if (stateMachine.IsNull())
                throw new ArgumentNullException(nameof(stateMachine));
            if (stateFields.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(stateFields), stateFields);
            if (states.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(states), states);
            if (stateFields.Length != states.Length)
                throw new ArgumentException("Arrays must be the same length.");

            FieldInfo field;
            IState state;
            for (int i = 0; i < stateFields.Length; i++)
            {
                field = stateFields[i];
                state = states.First(x => x.GetType() == field.FieldType);

                field.SetValue(stateMachine, state);
            }
        }

        public static IFactory<Type, IState>? FindFactoryInFields(
            IStateMachine stateMachine)
        {
            if (stateMachine.IsNull())
                throw new ArgumentNullException(nameof(stateMachine));

            return (from x in stateMachine.GetType().ForceGetFields(BindingFlagsDefault.InstanceAll)
                    where x.FieldType.IsType<IFactory<Type, IState>>()
                    select (IFactory<Type, IState>?)x.GetValue(stateMachine) into f
                    where f.IsNotNull()
                    select f).FirstOrDefault();
        }

        public static IState[] CreateStates(IFactory<Type, IState> factory,
                                            Type[] stateTypes)
        {
            if (factory.IsNull())
                throw new ArgumentNullException(nameof(factory));
            if (stateTypes.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(stateTypes), stateTypes);

            return stateTypes.Select(x => factory.Create(x)).ToArray();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static FieldInfo[] GetStateFields(IStateMachine stateMachine)
        {
            if (stateMachine.IsNull())
                throw new ArgumentNullException(nameof(stateMachine));

            return (from x in stateMachine.GetType().ForceGetFields(BindingFlagsDefault.InstanceAll)
                    where x.FieldType.IsType<IState>() && x.FieldType != typeof(IState)
                    select x).ToArray();
        }
    }
}
