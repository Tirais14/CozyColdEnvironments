#nullable enable
using System;
using System.Reflection;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

namespace UTIRLib.Attributes.Metadata
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
