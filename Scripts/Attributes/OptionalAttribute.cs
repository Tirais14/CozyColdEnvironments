using System;

#nullable enable

namespace UTIRLib.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, 
        AllowMultiple = false,
        Inherited = true
        )]
    public class OptionalAttribute : Attribute
    {
    }
}