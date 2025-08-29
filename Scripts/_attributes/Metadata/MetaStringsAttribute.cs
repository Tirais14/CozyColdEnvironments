#nullable enable
using System;

namespace CCEnvs.Attributes.Metadata
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class MetaStringsAttribute : MetadataAttribute
    {
        public string[] Values => (string[])rawData;

        public MetaStringsAttribute(params string[] values) : base(values)
        {
        }
    }
}
