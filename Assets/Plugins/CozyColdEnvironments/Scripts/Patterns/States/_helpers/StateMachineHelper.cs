using System;
using System.Linq;
using System.Reflection;
using CCEnvs.Patterns.Factories;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;

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
            CC.Guard.IsNotNull(stateMachine, nameof(stateMachine));
            Guard.IsNotNull(stateFields, nameof(stateFields));
            Guard.IsNotNull(states, nameof(states));
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

            return (from field in stateMachine.GetType().Reflect().IncludeNonPublic().IncludeBaseTypes().Fields()
                    where field.FieldType.IsType<IFactory<Type, IState>>()
                    select (IFactory<Type, IState>?)field.GetValue(stateMachine) into value
                    where value.IsNotNull()
                    select value)
                    .FirstOrDefault();
        }

        public static IState[] CreateStates(IFactory<Type, IState> factory,
                                            Type[] stateTypes)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));
            Guard.IsNotNull(stateTypes, nameof(stateTypes));

            return stateTypes.Select(x => factory.Create(x)).ToArray();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static FieldInfo[] GetStateFields(IStateMachine stateMachine)
        {
            if (stateMachine.IsNull())
                throw new ArgumentNullException(nameof(stateMachine));

            return stateMachine.GetType()
                               .Reflect().IncludeNonPublic().IncludeBaseTypes().Fields()
                               .Where(x => x.FieldType.IsType<IState>() && x.FieldType != typeof(IState))
                               .ToArray();
        }
    }
}
