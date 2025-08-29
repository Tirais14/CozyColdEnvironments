#nullable enable
using System;

namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public class OptionalPropertyAttribute : OptionalAttribute
    {
    
    }
}
