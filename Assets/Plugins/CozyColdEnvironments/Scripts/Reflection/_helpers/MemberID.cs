using CCEnvs.Attributes;
using CCEnvs.Snapshots;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class MemberID
    {
        private static Dictionary<string, MemberInfo> declaringTypes = null!;

        private static bool isInstalled;

        public static bool TryResolveMember(
            string id,
            [NotNullWhen(true)] out MemberInfo? member
            )
        {
            Guard.IsNotNull(id, nameof(id));

            if (!isInstalled)
            {
                (from assembly in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                 select assembly.GetTypes() into types
                 from type in types
                 select type.GetNestedTypes().Prepend(type) into types
                 from type in types
                 select type.FindMembers(MemberTypes.All.ResetFlags(MemberTypes.NestedType, MemberTypes.TypeInfo), BindingFlagsDefault.All, (member, _) => member.IsDefined<MemberIDAttribute>(inherit: true), null) into members
                 from mem in members
                 select mem)
                .FirstOrDefault(member => member.GetCustomAttribute<MemberIDAttribute>(inherit: true).ID == id)
                .Is(out member);
            }

            return declaringTypes.TryGetValue(id, out member);
        }

        [OnInstallExecutable]
        private static void OnInstall(MemberInfo[] domainMembers)
        {
            declaringTypes =
                (from member in domainMembers.AsParallel()
                 where member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property
                 where member.IsDefined<MemberIDAttribute>(inherit: true)
                 select (member, id: member.GetCustomAttribute<MemberIDAttribute>(inherit: true).ID)
                 )
                 .ToDictionary(info => info.id, info => info.member);

            declaringTypes.TrimExcess();
            isInstalled = true;
        }
    }
}
