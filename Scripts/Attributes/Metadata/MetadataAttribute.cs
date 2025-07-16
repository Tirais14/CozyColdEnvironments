using System;

#nullable enable

namespace UTIRLib.Attributes.Metadata
{
    public abstract class MetadataAttribute : Attribute
    {
        protected readonly object rawData;

        protected MetadataAttribute(object rawData) => this.rawData = rawData;
    }
}