using CCEnvs.Collections;
using System;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Attributes.Metadata
{
    public static class MemberMetadata
    {
        public static IMetdataAttribute[] GetMetadata(this MemberInfo member,
                                              bool throwIfNotFound = true)
        {
            var attributes = member.GetCustomAttributes().OfType<IMetdataAttribute>().ToArray();

            if (attributes.IsNullOrEmpty())
            {
                if (throwIfNotFound)
                    throw CC.ThrowHelper.MetadataNotFound(member);
                else
                    return Array.Empty<IMetdataAttribute>();
            }

            return attributes;
        }
        public static T[] GetMetadata<T>(this MemberInfo member,
                                         bool throwIfNotFound = true)
            where T : IMetdataAttribute
        {
            var results = member.GetMetadata().OfType<T>().ToArray();

            if (results.IsNullOrEmpty())
            {
                if (throwIfNotFound)
                    throw CC.ThrowHelper.MetadataNotFound(member);
                else
                    return Array.Empty<T>();
            }

            return results;
        }
    }
}
