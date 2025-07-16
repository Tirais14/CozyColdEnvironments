using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

#nullable enable
namespace UTIRLib.Attributes.Metadata
{
    public static class MemberInfoExtensions
    {

        public static MetadataAttribute[] GetMetadata(this MemberInfo member, bool throwIfNotFound = true)
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
