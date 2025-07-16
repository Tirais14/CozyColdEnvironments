using System;

#nullable enable

namespace UTIRLib.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeRestrictionAttribute : Attribute
    {
        public Type RestrictionType { get; }

        public SerializeRestrictionAttribute(Type type) => RestrictionType = type;
    }
}