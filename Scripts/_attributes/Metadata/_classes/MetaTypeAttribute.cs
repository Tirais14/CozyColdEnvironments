using System;

#nullable enable

namespace CCEnvs.Attributes.Metadata
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MetaTypeAttribute : Attribute, IMetdataAttribute<Type>
    {
        public Type Value { get; }

        public MetaTypeAttribute(Type value)
        {
            Value = value;
        }
    }
}