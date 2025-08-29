using System;

#nullable enable

namespace CozyColdEnvironments.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, 
        AllowMultiple = false,
        Inherited = true
        )]
    public class OptionalAttribute : Attribute
    {
    }
}