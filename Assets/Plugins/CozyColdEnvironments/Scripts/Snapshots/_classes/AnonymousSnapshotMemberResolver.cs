using CCEnvs.Pools;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Caching;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Snapshots
{
    public class AnonymousSnapshotMemberResolver
    {
        private readonly static Dictionary<Type, MemberInfo[]> propertyMemberInfos = new();
        private readonly static Dictionary<Type, (MemberInfo Value, SnapshotConvertibleAttribute Attribute)[]> compositerPartMemberInfos = new();

        private readonly static object propertyMemberInfosGate = new();
        private readonly static object compositerPartMemberInfosGate = new();

        public static AnonymousSnapshotMember[] ResolveMembers(Type targetType)
        {
            CC.Guard.IsNotNull(targetType, nameof(targetType));

            using var members = ListPool<AnonymousSnapshotMember>.Shared.Get();

            if (TryResolveMembersThroughCache(targetType, members.Value))
                return members.Value.ToArray();

            var memberInfos = GetMemberInfos(targetType);

            var propMemberInfos = memberInfos.Where(memberInfo => memberInfo.IsDefined<SnapshotPropertyAttribute>(inherit: true)).ToArray();

            lock (propertyMemberInfosGate)
                propertyMemberInfos.Add(targetType, propMemberInfos);

            CreateProperties(
                members.Value,
                propMemberInfos
                );

            var compositerPartMemberInfos = memberInfos.Select(memberInfo => (Value: memberInfo, Attribute: memberInfo.GetCustomAttribute<SnapshotConvertibleAttribute>(inherit: true)))
                .Where(memberInfo => memberInfo.Attribute is not null)
                .ToArray();

            lock (compositerPartMemberInfosGate)
                AnonymousSnapshotMemberResolver.compositerPartMemberInfos.Add(targetType, compositerPartMemberInfos);

            CreateCompositeParts(
                members.Value,
                compositerPartMemberInfos
                );

            return members.Value.ToArray();
        }

        private static bool TryResolveMembersThroughCache(
            Type targetType,
            List<AnonymousSnapshotMember> members
            )
        {
            lock (propertyMemberInfosGate)
            {
                if (propertyMemberInfos.TryGetValue(targetType, out var memberInfos))
                {
                    CreateProperties(members, memberInfos);

                    lock (compositerPartMemberInfosGate)
                    {
                        if (compositerPartMemberInfos.TryGetValue(targetType, out var compositePartMemberInfos))
                            CreateCompositeParts(members, compositePartMemberInfos);
                    }

                    return true;
                }
            }

            return false;
        }

        private static void CreateProperties(
            List<AnonymousSnapshotMember> members,
            IEnumerable<MemberInfo> memberInfos
            )
        {
            AnonymousSnapshotProperty prop;

            foreach (var memberInfo in memberInfos)
            {
                if (memberInfo is FieldInfo fieldInfo)
                    prop = new AnonymousSnapshotProperty(fieldInfo);
                else
                    prop = new AnonymousSnapshotProperty((PropertyInfo)memberInfo);

                members.Add(prop);
            }
        }

        private static bool TryCreateCompositePartByFieldInfo(
            FieldInfo fieldInfo,
            SnapshotConvertibleAttribute attribute,
            [NotNullWhen(true)] out AnonymousSnapshotCompositePart? part
            )
        {
            part = null;

            ISnapshot? snapshot;

            if (attribute.SnapshotType == null || attribute.SnapshotType.IsType<AnonymousSnapshot>())
                snapshot = new AnonymousSnapshot(fieldInfo.FieldType);
            else if (Snapshot.GetEmptyConstructor(attribute.SnapshotType, throwIfNotFound: false).IsNotNull(out var ctor))
            {
                try
                {
                    snapshot = (ISnapshot)ctor.Invoke(CC.EmptyArguments);
                }
                catch (Exception ex)
                {
                    typeof(AnonymousSnapshotMemberResolver).PrintException(ex);
                    return false;
                }
            }
            else
            {
                typeof(AnonymousSnapshotMemberResolver).PrintException(new InvalidOperationException($"Cannot find empty constructor. Type: {attribute.SnapshotType}"));
                return false;
            }

            part = new AnonymousSnapshotCompositePart(fieldInfo, snapshot);

            return part != null;
        }

        private static bool TryCreateCompositePartByPropInfo(
            PropertyInfo propInfo,
            SnapshotConvertibleAttribute attribute,
            [NotNullWhen(true)] out AnonymousSnapshotCompositePart? part
            )
        {
            part = null;

            ISnapshot? snapshot;

            if (attribute.SnapshotType == null)
                snapshot = new AnonymousSnapshot(propInfo.PropertyType);
            else if (Snapshot.GetEmptyConstructor(attribute.SnapshotType, throwIfNotFound: false).IsNotNull(out var ctor))
            {
                try
                {
                    snapshot = (ISnapshot)ctor.Invoke(CC.EmptyArguments);
                }
                catch (Exception ex)
                {
                    typeof(AnonymousSnapshotMemberResolver).PrintException(ex);
                    return false;
                }
            }
            else
            {
                typeof(AnonymousSnapshotMemberResolver).PrintException(new InvalidOperationException($"Cannot find empty constructor. Type: {attribute.SnapshotType}"));
                return false;
            }

            part = new AnonymousSnapshotCompositePart(propInfo, snapshot);

            return part != null;
        }

        private static void CreateCompositeParts(
            List<AnonymousSnapshotMember> members,
            IEnumerable<(MemberInfo Value, SnapshotConvertibleAttribute Attribute)> memberInfos
            )
        {
            foreach (var memberInfo in memberInfos)
            {
                if (memberInfo.Value is FieldInfo fieldInfo
                    &&
                    !TryCreateCompositePartByFieldInfo(fieldInfo, memberInfo.Attribute, out var part))
                {
                    continue;
                }
                else if (!TryCreateCompositePartByPropInfo((PropertyInfo)memberInfo.Value, memberInfo.Attribute, out part))
                    continue;

                members.Add(part);
            }
        }

        private static MemberInfo[] GetMemberInfos(Type type)
        {
            return type.FindMembers(
                MemberTypes.Field | MemberTypes.Property,
                BindingFlagsDefault.InstanceAll,
                (member, _) =>
                {
                    return member.IsDefined<SnapshotPropertyAttribute>(inherit: true)
                           ||
                           member.IsDefined<SnapshotConvertibleAttribute>(inherit: true);
                },
                null
                );
        }
    }
}
