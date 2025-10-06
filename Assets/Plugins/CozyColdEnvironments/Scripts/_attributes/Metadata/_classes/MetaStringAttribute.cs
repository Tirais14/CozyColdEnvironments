using System;

#nullable enable

namespace CCEnvs.Attributes.Metadata
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public sealed class MetaStringAttribute : Attribute, IMetdataAttribute<string>
    {
        public string Value { get; }

        public MetaStringAttribute(string value)
        {
            Value = value;
        }

        public MetaStringAttribute()
            :
            this(string.Empty)
        {
        }
    }
}