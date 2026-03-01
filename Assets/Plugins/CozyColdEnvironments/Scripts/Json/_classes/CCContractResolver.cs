using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Json
{
    public class CCContractResolver : DefaultContractResolver
    {
        public Dictionary<Type, List<string>> IgnoredTypeMemberNames { get; set; } = new();

        public StringMatchSettings IgnoreTypeMemberNamesMatchSettings { get; set; } =
            StringMatchSettings.Partial
            |
            StringMatchSettings.Ordinal
            |
            StringMatchSettings.IgnoreCase;

        public CCContractResolver()
        {
        }

        //protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        //{
        //    var serializableMembers = base.GetSerializableMembers(objectType);

        //    if (IgnoredTypeMemberNames.IsNullOrEmpty())
        //        return serializableMembers;

        //    MemberInfo member;

        //    for (int i = 0; i < serializableMembers.Count; i++)
        //    {
        //        member = serializableMembers[i];

        //        foreach (var ignoredTypeMemberName in IgnoredTypeMemberNames)
        //        {
        //            if (ignoredTypeMemberName.Key != typeof(void)
        //                &&
        //                objectType.IsNotType(ignoredTypeMemberName.Key))
        //            {
        //                continue;
        //            }

        //            foreach (var excludeMemberName in ignoredTypeMemberName.Value)
        //            {
        //                if (member.Name.Match(excludeMemberName, IgnoreTypeMemberNamesMatchSettings))
        //                    serializableMembers.RemoveAt(i);
        //            }
        //        }
        //    }

        //    return serializableMembers;
        //}
    }
}
