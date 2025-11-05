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
        private enum InjectFrom
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
            FieldInfo[] fields = source.ReflectQuery()
                                       .NonPublic()
                                       .IncludeBaseTypes()
                                       .Fields()
                                       .ToArray();

            return fields.Where(x => x.IsDefined<GetComponentAttribute>())
                         .Select(x => (x, x.GetCustomAttribute<GetComponentAttribute>()));
        }

        /// <exception cref="GameObjectNotFoundException"></exception>
        private static void SetField(Component source,
            FieldInfo field,
            GetComponentAttribute attribute,
            InjectFrom injectFrom)
        {
            Type injectType = field.FieldType;

            bool isSupportedValueType = injectType.IsGenericType
                                        &&
                                        injectType.GetGenericTypeDefinition()
                                                  .IsAnyType(typeof(Maybe<>),
                                                             typeof(MaybeStruct<>),
                                                             typeof(Catched<>),
                                                             typeof(IfElse<>));

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

            object? foundComponent;
            try
            {
                foundComponent = injectFrom switch
                {
                    InjectFrom.Self => source.FindFor().Component(injectType).Strict(),

                    InjectFrom.Parent => source.FindFor()
                                               .InParent()
                                               .IncludeInactive()
                                               .Component(injectType)
                                               .Strict(),

                    InjectFrom.Child => source.FindFor()
                                              .InChildren()
                                              .IncludeInactive()
                                              .Component(injectType)
                                              .Strict(),

                    InjectFrom.GameObject => source.FindFor()
                                                   .InChildren()
                                                   .IncludeInactive()
                                                   .Name(attribute.Name)
                                                   .GameObject()
                                                   .Strict()
                                                   .FindFor()
                                                   .Component(injectType)
                                                   .Strict(),

                    _ => throw new InvalidOperationException(injectFrom.ToString())
                };
            }
            catch (Exception ex)
            {
                if (attribute.IsOptional
                    ||
                    field.FieldType.IsType<IConditional>())
                {
                    typeof(ComponentInjector).PrintLog($"Not injected in component: {source.GetType().GetFullName()}; field: {BackingField.HumanizeName(field)}; fieldType: {field.FieldType.GetFullName()}; type: {injectType.GetFullName()}. Is optional and not found.");
                    return;
                }

                typeof(ComponentInjector).PrintException(ex);
                return;
            }

            if (foundComponent is IConditional conditional)
                foundComponent = conditional.Access();

            if (injectType.IsNotType(field.FieldType))
            {
                var converted = foundComponent!.MutateType(field.FieldType);

                field.SetValue(source, converted);
            }
            else
                field.SetValue(source, foundComponent);

            typeof(ComponentInjector).PrintLog($"Injected in component: {source.GetType().GetFullName()}; field: {BackingField.HumanizeName(field)}; fieldType: {field.FieldType.GetFullName()}; type: {injectType.GetFullName()}.");
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
                            InjectFrom.Self);
                        break;
                    case GetByParentAttribute:
                        SetField(
                            source,
                            field,
                            attribute: attribute,
                            InjectFrom.Parent);
                        break;
                    case GetByChildrenAttribute byChild:
                        byChild.Name.Maybe().Match(
                            _ => SetField(
                            source,
                            field,
                            attribute,
                            InjectFrom.GameObject),

                            () => SetField(
                            source,
                            field,
                            attribute,
                            InjectFrom.Child));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}