using CCEnvs.Attributes;
using CCEnvs.Caching;
using CCEnvs.Diagnostics;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using CCEnvs.UnityX.Components;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.UnityX.ComponentInjections
{
    /// <summary>
    /// Collect all components by <see cref="GetComponentAttribute"/> and setts to it fields or properties.
    /// <see cref="CCBehaviour"/> already contains implements this.
    /// </summary>
    public static class ComponentInjector
    {
        public class Debug { }

        private readonly static Cache<Type, IReadOnlyList<InjectableFieldInfo>> cachedInjectableFields = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        public static void Inject(Component target)
        {
            List<string>? debugInfo = null;

            if (CCDebug<Debug>.IsEnabled)
                debugInfo = new List<string>(64);

            IReadOnlyList<InjectableItem> injectableItems = GetInjectableItems(target, debugInfo);

            try
            {
                foreach (var injectable in injectableItems)
                    TryInject(injectable, debugInfo);
            }
            finally
            {
                PrintDebugInfo(target, debugInfo);
            }
        }

        public static void InjectAwake(Component target)
        {
            List<string>? debugInfo = null;

            if (CCDebug<Debug>.IsEnabled)
                debugInfo = new List<string>(64);

            IReadOnlyList<InjectableItem> injectableItems = GetInjectableItems(target, debugInfo);

            try
            {
                foreach (var injectable in injectableItems)
                    if (injectable.ResolveFindMode() == FindMode.Self)
                        TryInject(injectable, debugInfo);
            }
            finally
            {
                PrintDebugInfo(target, debugInfo);
            }
        }

        public static void InjectStart(Component target)
        {
            List<string>? debugInfo = null;

            if (CCDebug<Debug>.IsEnabled)
                debugInfo = new List<string>(64);

            IReadOnlyList<InjectableItem> injectableItems = GetInjectableItems(target, debugInfo);

            try
            {
                foreach (var injectable in injectableItems)
                    if (injectable.ResolveFindMode() != FindMode.Self)
                        TryInject(injectable, debugInfo);
            }
            finally
            {
                PrintDebugInfo(target, debugInfo);
            }
        }

        private static IReadOnlyList<InjectableItem> GetInjectableItems(
           Component target,
           List<string>? debugInfo
           )
        {
            var targetType = target.GetType();

            try
            {
                InjectableItem injectItem;

                var fields = GetFields(target, debugInfo);
                var injectableItems = new List<InjectableItem>(fields.Count);

                foreach (var (field, attribute) in fields)
                {
                    injectItem = new InjectableItem(
                        target,
                        field,
                        attribute
                        );

                    injectableItems.Add(injectItem);
                }

                injectableItems.TrimExcess();

                return injectableItems;
            }
            catch (Exception ex)
            {
                typeof(ComponentInjector).PrintException(ex);
                return Array.Empty<InjectableItem>();
            }
        }

        private static IReadOnlyList<InjectableFieldInfo> GetFields(
            Component target,
            List<string>? debugInfo
            )
        {
            var type = target.GetType();

            if (cachedInjectableFields.TryGetValue(type, out IReadOnlyList<InjectableFieldInfo>? cachedFields))
                return cachedFields;

            var fields = new List<InjectableFieldInfo>(64);

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
                    if (CCDebug<Debug>.IsEnabled && debugInfo is not null)
                        debugInfo.Add($"Cannot inject readonly field. Field: {field}; Type: {type}");

                    continue;
                }

                fields.Add(new InjectableFieldInfo(field, attribute));
            }

            fields.TrimExcess();

            if (cachedInjectableFields.TryAdd(type, fields, out ICacheEntry<IReadOnlyList<InjectableFieldInfo>>? cacheEntry))
                cacheEntry.ExpirationTimeRelativeToNow = 5.Minutes();

            return fields;
        }

        private static bool TryInject(InjectableItem item, List<string>? debugInfo)
        {
            if (item.Value.IsNotNull())
            {
                if (CCDebug<Debug>.IsEnabled && debugInfo is not null)
                    debugInfo.Add($"Item already have value. Item: {item}");

                return false;
            }

            if (!TryGetDependecy(item, out var dep))
            {
                if (CCDebug<Debug>.IsEnabled && debugInfo is not null)
                    debugInfo.Add($"Cannot find dependency. Item: {item}");

                return false;
            }

            item.Inject(dep);

            if (CCDebug<Debug>.IsEnabled && debugInfo is not null)
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

            query.nameFilterSettings = item.Attribute.NameMatchSettings;

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

        private static void PrintDebugInfo(Component target, List<string>? debugInfo)
        {
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

        [OnInstallExecutable]
        private static void OnInstall()
        {
            cachedInjectableFields.Clear();
        }
    }
}