using CCEnvs.Attributes.Metadata;
using System;

namespace CCEnvs.Attributes.Metadata
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class MetaNumberAttribute : Attribute, IMetdataAttribute<long>
    {
        public long Value { get; }

        public MetaNumberAttribute(long value)
        {
            Value = value;
        }

        public MetaNumberAttribute() : this(default)
        {
        }
    }
}
