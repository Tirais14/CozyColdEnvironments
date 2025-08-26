using System;
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
        public static void CreateStatesByFactory(object stateMachine,
                                                 IFactory<Type, IState>? factory = null)
        {
            if (stateMachine.IsNull())
                throw new ArgumentNullException(nameof(stateMachine));

            FieldInfo[] fields = stateMachine.GetType()
                .ForceGetFields(BindingFlagsDefault.InstanceAll);

            if (factory.IsNull())
                factory = (IFactory<Type, IState>)fields.First(
                    x => x.FieldType.IsType<IFactory<Type, IState>>())
                        .GetValue(stateMachine);

            if (fields.IsEmpty())
                throw new ArgumentException($"{stateMachine.GetTypeName()} doesn't contain any state field.");

            fields = GetStateFields(fields);
            foreach (var field in fields)
                field.SetValue(stateMachine, factory.Create(field.FieldType));
        }

        private static FieldInfo[] GetStateFields(FieldInfo[] fields)
        {
            return fields.Where(x => x.FieldType.IsType<IState>() && x.FieldType != typeof(IState))
                         .ToArray();
        }
    }
}
