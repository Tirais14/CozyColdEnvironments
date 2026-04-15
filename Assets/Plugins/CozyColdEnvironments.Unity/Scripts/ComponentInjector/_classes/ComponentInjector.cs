using CCEnvs.Caching;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private readonly static Cache<Type, (FieldInfo Value, GetComponentAttribute Attribute)[]> cachedTypeFields = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        public static void Inject(Component target)
        {
            List<string>? debugInfo = null;

            if (CCDebug.Instance.IsEnabled && CCDebug.IsTypeEnabled(typeof(ComponentInjector)))
                debugInfo = ListPool<string>.Shared.Get().Value;   

            var injectableItems = GetInjectableItems(target, debugInfo);

            try
            {
                foreach (var injectable in injectableItems.Value)
                    TryInject(injectable, debugInfo);
            }
            finally
            {
                injectableItems.Dispose();

                if (debugInfo is not null)
                {
                    using var sb = StringBuilderPool.Shared.Get();

                    sb.Value.AppendLine($"Injectable type: {target.GetType()}; name: {target.name}");
                    sb.Value.AppendLine("{");

                    foreach (var str in debugInfo)
                        sb.Value.AppendLine("\t" + str);

                    sb.Value.Append("}");

                    var debugString = sb.Value.ToString();

                    typeof(ComponentInjector).PrintLog(debugString);

                    ListPool<string>.Shared.Return(debugInfo);
                }
            }
        }

        private static PooledObject<List<InjectableItem>> GetInjectableItems(Component target, List<string>? debugInfo)
        {
            var injectItems = ListPool<InjectableItem>.Shared.Get();

            try
            {
                var type = target.GetType();

                InjectableItem injectItem;

                foreach (var (field, attribute) in GetFields(target, debugInfo))
                {
                    injectItem = new InjectableItem(
                        target,
                        field,
                        attribute
                        );

                    injectItems.Value.Add(injectItem);
                }

                return injectItems;
            }
            catch (Exception ex)
            {
                typeof(ComponentInjector).PrintException(ex);
                injectItems.Dispose();

                return PooledObject<List<InjectableItem>>.Default;
            }
        }

        private static (FieldInfo Value, GetComponentAttribute Attribute)[] GetFields(Component target, List<string>? debugInfo)
        {
            var type = target.GetType();

            if (cachedTypeFields.TryGetValue(type, out var fields))
                return fields;

            using var fieldList = ListPool<(FieldInfo Value, GetComponentAttribute Attribute)>.Shared.Get();

            var typeHierarchy = Loops.BreadthFirstSearch(type,
                static (type, loopState) =>
                {
                    if (type == TypeofCache<MonoBehaviour>.Type || type == TypeofCache<Component>.Type)
                    {
                        loopState.Break = true;
                        return null;
                    }

                    return type.BaseType;
                });

            var relfected = typeHierarchy.SelectMany(type =>
            {
                return type.FindMembers(
                    MemberTypes.Field,
                    BindingFlagsDefault.InstanceAll | BindingFlags.DeclaredOnly,
                    (member, _) => member.IsDefined<GetComponentAttribute>(inherit: true),
                    null
                    );
            });

            foreach (var field in relfected.OfType<FieldInfo>())
            {
                if (field.GetCustomAttribute<GetComponentAttribute>().IsNull(out var attribute))
                    continue;

                if (field.IsInitOnly)
                {
                    if (CCDebug.Instance.IsEnabled && debugInfo is not null)
                        debugInfo.Add($"Cannot inject readonly field. Field: {field}; Type: {type}");

                    continue;
                }

                fieldList.Value.Add((field, attribute));
            }

            fields = fieldList.Value.ToArray();

            if (cachedTypeFields.TryAdd(type, fields, out var entry))
                entry.ExpirationTimeRelativeToNow = 20.Minutes();

            return fields;
        }

        private static bool TryInject(InjectableItem item, List<string>? debugInfo)
        {
            if (item.Value.IsNotNull())
            {
                if (CCDebug.Instance.IsEnabled && debugInfo is not null)
                    debugInfo.Add($"Item already have value. Item: {item}");

                return false;
            }

            if (!TryGetDependecy(item, out var dep))
            {
                if (CCDebug.Instance.IsEnabled && debugInfo is not null)
                    debugInfo.Add($"Cannot find dependency. Item: {item}");

                return false;
            }

            item.Inject(dep);

            if (CCDebug.Instance.IsEnabled && debugInfo is not null)
                debugInfo.Add($"Item injected. Item: {item}; Value: {(dep.IsNull() ? "null" : dep)}");

            return true;
        }

        private static bool TryGetDependecy(InjectableItem item, [NotNullWhen(true)] out object? result)
        {
            var query = item.Target.Q()
                .IncludeInactive();

            query.findMode = item.ResolveFindMode();

            if (item.Attribute.NameFilter.IsNotNull(out var goName))
                query.WithName(goName);

            query.nameMatchSettings = item.Attribute.NameMatchSettings;

            if (item.Attribute.TagFilter.IsNotNull(out var goTag))
                query.WithTag(goTag);

            var seekingType = item.GetValueType();

            var findResult = query.Component(seekingType);

            if (item.Attribute.IsOptional)
                result = findResult.Lax().GetValue();
            else
                result = findResult.Strict();

            return result.IsNotNull();
        }
       

        private struct InjectableItem
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
                    _ => throw new InvalidOperationException(),
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
                return $"(Member: {Field.FieldType.Name}.{Field.Name}; declaring type: {Field.DeclaringType.Name})";
            }
        }

        ///// <exception cref="ArgumentNullException"></exception>
        //public static void Inject(Component target)
        //{
        //    Guard.IsNotNull(target, nameof(target));

        //    var fields = GetAttributedFields(target);

        //    if (fields.IsNotEmpty())
        //        SetFields(target, fields);
        //}

        //private static IEnumerable<(FieldInfo, GetComponentAttribute)> GetAttributedFields(
        //    Component source)
        //{
        //    FieldInfo[] fields = source.Reflect()
        //                               .IncludeNonPublic()
        //                               .IncludeBaseTypes()
        //                               .Fields()
        //                               .ToArray();

        //    return fields.Where(x => x.IsDefined<GetComponentAttribute>(inherit: true))
        //                 .Select(x => (x, x.GetCustomAttribute<GetComponentAttribute>(inherit: true)));
        //}

        ///// <exception cref="GameObjectNotFoundException"></exception>
        //private static void SetField(Component source,
        //    FieldInfo field,
        //    GetComponentAttribute attribute,
        //    InjectFrom injectFrom)
        //{
        //    Type injectType = field.FieldType;

        //    bool isSupportedValueType = injectType.IsGenericType
        //                                &&
        //                                injectType.GetGenericTypeDefinition()
        //                                          .IsAnyType(typeof(Maybe<>),
        //                                                     typeof(IfElse<>));

        //    if (isSupportedValueType)
        //        injectType = injectType.GetGenericArguments()[0];

        //    if (injectType.IsValueType)
        //    {
        //        typeof(ComponentInjector).PrintError($"Unsupported inject type: {injectType.GetFullName()}.");
        //        return;
        //    }

        //    if (field.GetValue(source) is object fieldValue)
        //    {
        //        if (fieldValue.Is<IConditional>(out var cond) && cond.IsSome)
        //        {
        //            if (CCDebug.Instance.IsEnabled)
        //                CCDebug.Instance.PrintLog($"Field {field.FieldType.GetName()} is {field.ReflectedType.GetName()} already setted.");

        //            return;
        //        }
        //    }

        //    object? foundComponent;
        //    try
        //    {
        //        foundComponent = injectFrom switch
        //        {
        //            InjectFrom.Self => source.QueryTo().Component(injectType).Strict(),

        //            InjectFrom.Parent => source.QueryTo()
        //                                       .FromParents()
        //                                       .IncludeInactive()
        //                                       .Component(injectType)
        //                                       .Strict(),

        //            InjectFrom.Child => source.QueryTo()
        //                                      .FromChildrens()
        //                                      .IncludeInactive()
        //                                      .Component(injectType)
        //                                      .Strict(),

        //            InjectFrom.GameObject => source.QueryTo()
        //                                           .FromChildrens()
        //                                           .IncludeInactive()
        //                                           .WithName(attribute.ObjectName)
        //                                           .GameObject()
        //                                           .Strict()
        //                                           .QueryTo()
        //                                           .Component(injectType)
        //                                           .Strict(),

        //            _ => throw new InvalidOperationException(injectFrom.ToString())
        //        };
        //    }
        //    catch (Exception)
        //    {
        //        if (attribute.IsOptional
        //            ||
        //            field.FieldType.IsType<IConditional>())
        //        {
        //            printNotInjectedLog();
        //            return;
        //        }

        //        throw;
        //    }

        //    if (foundComponent is IConditional conditional)
        //    {
        //        if (conditional.IsNone)
        //        {
        //            printNotInjectedLog();
        //            return;
        //        }

        //        foundComponent = conditional.GetValueUnsafe();
        //    }

        //    if (injectType.IsNotType(field.FieldType))
        //    {
        //        var converted = foundComponent.MutateType(field.FieldType);

        //        field.SetValue(source, converted);
        //    }
        //    else
        //        field.SetValue(source, foundComponent);

        //    if (CCDebug.Instance.IsEnabled)
        //        typeof(ComponentInjector).PrintLog($"Injected in component: {source.GetType().GetFullName()}; field: {BackingField.HumanizeName(field)}; fieldType: {field.FieldType.GetFullName()}; type: {injectType.GetFullName()}.");

        //    void printNotInjectedLog()
        //    {
        //        if (CCDebug.Instance.IsEnabled)
        //            typeof(ComponentInjector).PrintLog($"Not injected in component: {source.GetType().GetFullName()}; field: {BackingField.HumanizeName(field)}; fieldType: {field.FieldType.GetFullName()}; type: {injectType.GetFullName()}. Is optional and not found.");
        //    }
        //}

        //private static void SetFields(Component source,
        //    IEnumerable<(FieldInfo field, GetComponentAttribute attribute)> fields)
        //{
        //    foreach (var (field, attribute) in fields)
        //    {
        //        switch (attribute)
        //        {
        //            case GetBySelfAttribute:
        //                SetField(
        //                    source,
        //                    field,
        //                    attribute: attribute,
        //                    InjectFrom.Self);
        //                break;
        //            case GetByParentAttribute:
        //                SetField(
        //                    source,
        //                    field,
        //                    attribute: attribute,
        //                    InjectFrom.Parent);
        //                break;
        //            case GetByChildrenAttribute byChild:
        //                byChild.ObjectName.Maybe().Do(
        //                    _ => SetField(
        //                    source,
        //                    field,
        //                    attribute,
        //                    InjectFrom.GameObject),

        //                    () => SetField(
        //                    source,
        //                    field,
        //                    attribute,
        //                    InjectFrom.Child));
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}
    }
}