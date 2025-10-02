using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Factory;
using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public static class StateMachineHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EmptyCollectionArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void InjectStates(IStateMachine stateMachine,
                                        FieldInfo[] stateFields,
                                        IState[] states)
        {
            CC.Guard.NullArgument(stateMachine, nameof(stateMachine));
            CC.Guard.CollectionArgument(stateFields, nameof(stateFields));
            CC.Guard.CollectionArgument(states, nameof(states));
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
            CC.Guard.NullArgument(factory, nameof(factory));
            CC.Guard.CollectionArgument(stateTypes, nameof(stateTypes));

            return stateTypes.Select(x => factory.Create(x)).ToArray();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static FieldInfo[] GetStateFields(IStateMachine stateMachine)
        {
            if (stateMachine.IsNull())
                throw new ArgumentNullException(nameof(stateMachine));

            return stateMachine.GetType()
                               .ForceGetFields(BindingFlagsDefault.InstanceAll)
                               .Where(x => x.FieldType.IsType<IState>() && x.FieldType != typeof(IState))
                               .ToArray();
        }
    }
}
