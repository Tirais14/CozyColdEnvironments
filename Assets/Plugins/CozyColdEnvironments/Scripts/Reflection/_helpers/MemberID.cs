using CCEnvs.Attributes;
using CommunityToolkit.Diagnostics;
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

        public static bool TryResolveMember(
            string id, 
            [NotNullWhen(true)] out MemberInfo member
            )
        {
            Guard.IsNotNull(id, nameof(id));

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
        }
    }
}
