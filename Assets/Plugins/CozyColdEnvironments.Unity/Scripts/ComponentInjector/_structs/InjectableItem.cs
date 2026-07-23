using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Reflection;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.ComponentInjections
{
    public struct InjectableItem
    {
        public Component Target;

        public FieldInfo Field;

        public GetComponentAttribute Attribute;

        public readonly object? Value => Field.GetValue(Target);

        public InjectableItem(
            Component target,
            FieldInfo field,
            GetComponentAttribute attribute
            )
        {
            Target = target;
            Field = field;
            Attribute = attribute;
        }

        public readonly void Inject(object? obj)
        {
            if (obj.IsNotNull() && obj.IsNotInstanceOfType(Field.FieldType))
                obj = obj.MutateType(Field.FieldType);

            Field.SetValue(Target, obj);
        }

        public readonly FindMode ResolveFindMode()
        {
            return Attribute switch
            {
                GetBySelfAttribute => FindMode.Self,
                GetByChildrenAttribute => FindMode.InChilds,
                GetByParentAttribute => FindMode.InParents,
                _ => throw CC.ThrowHelper.InvalidOperationException(Attribute.GetType())
            };
        }

        public readonly Type GetValueType()
        {
            if (Field.FieldType.IsType(typeof(Maybe<>), TypeMatchingSettings.ByBaseGenericTypeDefinition))
                return Field.FieldType.GetGenericArguments()[0];

            return Field.FieldType;
        }

        public readonly override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Target), Target)
                .AddProperty(nameof(Field), Field.Name)
                .AddProperty(nameof(Field.DeclaringType), Field.DeclaringType)
                .AddProperty(nameof(Attribute), Attribute)
                .AddProperty(nameof(Value), Value)
                .ToStringAndDispose();
        }
    }
}
