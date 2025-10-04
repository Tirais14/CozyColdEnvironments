#nullable enable

using System;

namespace CCEnvs.Attributes.Metadata
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public sealed class MetaStringsAttribute : Attribute, IMetdataAttribute<string[]>
    {
        public string[] Value { get; }

        public MetaStringsAttribute(params string[] values)
        {
            Value = values;
        }
    }
}