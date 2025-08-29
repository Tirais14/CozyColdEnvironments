using System;
using System.Linq;
using System.Reflection;
using CCEnvs.Reflection.Cached;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class MemberInfoExtensions
    {
        public static T? TryCacheMember<T>(this T? value, out bool result)
            where T : MemberInfo
        {
            if (value is null)
            {
                result = false;
                return null;
            }

            result = TypeCache.TryCacheMember(value);
            return value;
        }
        public static T? TryCacheMember<T>(this T? value)
            where T : MemberInfo
        {
            return value.TryCacheMember(out _);
        }

        public static bool IsDefined<T>(this MemberInfo value)
            where T : Attribute
        {
            return value.IsDefined(typeof(T));
        }
        public static bool IsDefined<T>(this MemberInfo value, bool inherit)
            where T : Attribute
        {
            return value.IsDefined(typeof(T), inherit);
        }

        public static bool IsDefinedAny(this MemberInfo value,
                                        params Type[] attributeTypes)
        {
            return attributeTypes.Any(x => value.IsDefined(x));
        }
        public static bool IsDefinedAny(this MemberInfo value,
                                        bool inherit,
                                        params Type[] attributeTypes)
        {
            return attributeTypes.Any(x => value.IsDefined(x, inherit));
        }
    }
}

namespace CCEnvs.Attributes.Metadata
{
    public static class MemberInfoExtensions
    {
        public static MetadataAttribute[] GetMetadata(this MemberInfo member,
                                                      bool throwIfNotFound = true)
        {
            var attributes = member.GetCustomAttributes<MetadataAttribute>().ToArray();

            if (attributes.IsNullOrEmpty())
            {
                if (throwIfNotFound)
                    throw new MetadataAttributeNotFoundException(member);
                else
                    return Array.Empty<MetadataAttribute>();
            }

            return attributes;
        }
    }
}
