#nullable enable
using System;
using System.Reflection;
using CozyColdEnvironments.Diagnostics;
using CozyColdEnvironments.Extensions;

namespace CozyColdEnvironments.Attributes.Metadata
{
    public class MetadataAttributeNotFoundException : TirLibException
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
