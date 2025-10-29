using CCEnvs.Conversations;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Diagnostics;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.Unity.Injections
{
    /// <summary>
    /// Collect all components by <see cref="GetComponentAttribute"/> and setts to it fields or properties.
    /// <see cref="CCBehaviour"/> already contains implements this.
    /// </summary>
    public static class ComponentInjector
    {
        private enum InejctFrom
        {
            Self,
            Child,
            Parent,
            GameObject
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static void Inject(Component target)
        {
            Guard.IsNotNull(target, nameof(target));

            var fields = GetAttributedFields(target);

            if (fields.IsNotEmpty())
                SetFields(target, fields);
        }

        private static IEnumerable<(FieldInfo, GetComponentAttribute)> GetAttributedFields(
            Component source)
        {
            FieldInfo[] fields = source.GetType()
                                       .ForceGetFields(BindingFlagsDefault.InstanceAll);

            return fields.Where(x => x.IsDefined<GetComponentAttribute>())
                         .Select(x => (x, x.GetCustomAttribute<GetComponentAttribute>()));
        }

        /// <exception cref="GameObjectNotFoundException"></exception>
        private static void SetField(Component source,
            FieldInfo field,
            GetComponentAttribute attribute,
            InejctFrom injectFrom)
        {
            Type injectType = field.FieldType;

            bool isSupportedValueType = injectType.IsGenericType
                                        &&
                                        injectType.GetGenericTypeDefinition()
                                                  .IsAnyType(typeof(Maybe<>),
                                                             typeof(MaybeStruct<>),
                                                             typeof(Catched<>));

            if (isSupportedValueType)
            {
                injectType = injectType.GetGenericArguments()[0];
            }

            if (injectType.IsValueType)
            {
                typeof(ComponentInjector).PrintError($"Unsupported field type: {injectType.GetFullName()}.");
                return;
            }
            if (field.GetValue(source).IsNotDefault())
            {
                CCDebug.PrintLog($"Field {field.FieldType.GetName()} is {field.ReflectedType.GetName()} already setted.");
                return;
            }

            Maybe<GameObject> fromGameObject = attribute.GameObjectName.Map(x => source.gameObject.Find(x)!);
            Maybe<object> foundComponent = injectFrom switch
            {
                InejctFrom.Parent => source.GetAssignedObjectInParent(injectType, includeInactive: true),
                InejctFrom.Child => source.GetAssignedObjectInChildren(injectType, includeInactive: true),
                InejctFrom.GameObject => fromGameObject.Match(
                    go => go.GetAssignedObject(injectType),
                    () => typeof(ComponentInjector).PrintError($"Cannot find {nameof(GameObject)}: {attribute.GameObjectName.Access()}.")),
                _ => source.GetAssignedObject(injectType)
            };

            if ((attribute.IsOptional
                ||
                field.FieldType.IsType<IConditional>())
                &&
                foundComponent.IsNone)
            {
                typeof(ComponentInjector).PrintLog($"Not injected in component: {source.GetType().GetFullName()}; field: {BackingField.GetHumanizedName(field)}; fieldType: {field.FieldType.GetFullName()}; type: {injectType.GetFullName()}. Is optional and not found.");
                return;
            }

            ComponentNotFoundException.ThrowIfNull(
                foundComponent.Access(),
                field.FieldType,
                source.gameObject);

            if (injectType.IsNotType(field.FieldType))
            {
                var converted = foundComponent.Map(x => x.MutateType(field.FieldType)).AccessUnsafe();

                field.SetValue(source, converted);
            }
            else
                field.SetValue(source, foundComponent
                     .AccessUnsafe());

            typeof(ComponentInjector).PrintLog($"Injected in component: {source.GetType().GetFullName()}; field: {BackingField.GetHumanizedName(field)}; fieldType: {field.FieldType.GetFullName()}; type: {injectType.GetFullName()}.");
        }

        private static void SetFields(Component source,
            IEnumerable<(FieldInfo field, GetComponentAttribute attribute)> fields)
        {
            foreach (var (field, attribute) in fields)
            {
                switch (attribute)
                {
                    case GetBySelfAttribute:
                        SetField(
                            source,
                            field,
                            attribute: attribute,
                            InejctFrom.Self);
                        break;
                    case GetByParentAttribute:
                        SetField(
                            source,
                            field,
                            attribute: attribute,
                            InejctFrom.Parent);
                        break;
                    case GetByChildrenAttribute byChild:
                        byChild.GameObjectName.Match(
                            _ => SetField(
                            source,
                            field,
                            attribute,
                            InejctFrom.GameObject),

                            () => SetField(
                            source,
                            field,
                            attribute,
                            InejctFrom.Child));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}