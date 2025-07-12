using System;

#nullable enable
namespace UTIRLib.Attributes.Metadata
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MetaTypeAttribute : MetadataAttribute
    {
        public Type Value => (Type)rawData;

        public MetaTypeAttribute(Type type) : base(type)
        {
        }
    }
}
