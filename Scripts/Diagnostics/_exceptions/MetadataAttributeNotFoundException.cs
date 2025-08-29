#nullable enable
using System;
using System.Reflection;
using CCEnvs.Diagnostics;
using CCEnvs.Extensions;

namespace CCEnvs.Attributes.Metadata
{
    public class MetadataAttributeNotFoundException : CCEException
    {
        public MetadataAttributeNotFoundException()
        {
        }

        public MetadataAttributeNotFoundException(MemberInfo member)
            : base($"{member.MemberType}: {member.Name}.")
        {
        }

        public MetadataAttributeNotFoundException(Type concreteType)
            : base($"{concreteType.Name.InsertWhitespacesByCase()}")
        {
        }
    }
}
