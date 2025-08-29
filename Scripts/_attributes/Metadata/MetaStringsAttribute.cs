#nullable enable
using System;

namespace CozyColdEnvironments.Attributes.Metadata
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
