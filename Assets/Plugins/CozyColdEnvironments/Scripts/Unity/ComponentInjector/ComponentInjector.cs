using CCEnvs.Collections;
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
            FieldInfo[] fields = source.Reflect()
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
                                                             typeof(IfElse<>));

            if (isSupportedValueType)
                injectType = injectType.GetGenericArguments()[0];

            if (injectType.IsValueType)
            {
                typeof(ComponentInjector).PrintError($"Unsupported inject type: {injectType.GetFullName()}.");
                return;
            }
            if (field.GetValue(source) is object fieldValue)
            {
                fieldValue = fieldValue.As<IConditional>().Map(x => x.GetValue()).Raw!;

                if (fieldValue.IsNotNull())
                {
                    CCDebug.Instance.PrintLog($"Field {field.FieldType.GetName()} is {field.ReflectedType.GetName()} already setted.");
                    return;
                }
            }

            object? foundComponent;
            try
            {
                foundComponent = injectFrom switch
                {
                    InjectFrom.Self => source.QueryTo().Component(injectType).Strict(),

                    InjectFrom.Parent => source.QueryTo()
                                               .FromParents()
                                               .IncludeInactive()
                                               .Component(injectType)
                                               .Strict(),

                    InjectFrom.Child => source.QueryTo()
                                              .FromChildrens()
                                              .IncludeInactive()
                                              .Component(injectType)
                                              .Strict(),

                    InjectFrom.GameObject => source.QueryTo()
                                                   .FromChildrens()
                                                   .IncludeInactive()
                                                   .WithName(attribute.ObjectName)
                                                   .GameObject()
                                                   .Strict()
                                                   .QueryTo()
                                                   .Component(injectType)
                                                   .Strict(),

                    _ => throw new InvalidOperationException(injectFrom.ToString())
                };
            }
            catch (Exception)
            {
                if (attribute.IsOptional
                    ||
                    field.FieldType.IsType<IConditional>())
                {
                    printNotInjectedLog();
                    return;
                }

                throw;
            }

            if (foundComponent is IConditional conditional)
            {
                if (conditional.IsNone)
                {
                    printNotInjectedLog();
                    return;
                }

                foundComponent = conditional.GetValueUnsafe();
            }

            if (injectType.IsNotType(field.FieldType))
            {
                var converted = foundComponent.MutateType(field.FieldType);

                field.SetValue(source, converted);
            }
            else
                field.SetValue(source, foundComponent);

            typeof(ComponentInjector).PrintLog($"Injected in component: {source.GetType().GetFullName()}; field: {BackingField.HumanizeName(field)}; fieldType: {field.FieldType.GetFullName()}; type: {injectType.GetFullName()}.");

            void printNotInjectedLog()
            {
                typeof(ComponentInjector).PrintLog($"Not injected in component: {source.GetType().GetFullName()}; field: {BackingField.HumanizeName(field)}; fieldType: {field.FieldType.GetFullName()}; type: {injectType.GetFullName()}. Is optional and not found.");
            }
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
                        byChild.ObjectName.Maybe().Do(
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