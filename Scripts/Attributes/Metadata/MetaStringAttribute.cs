using System;

#nullable enable

namespace UTIRLib.Attributes.Metadata
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class MetaStringAttribute : MetadataAttribute
    {
        public string Value => (string)rawData;

        public MetaStringAttribute(string value) : base(value)
        {
        }
    }
}