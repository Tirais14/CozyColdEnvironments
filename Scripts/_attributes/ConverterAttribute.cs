#nullable enable
using System;

namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.Method,
        AllowMultiple = false,
        Inherited = true)]
    public class ConverterAttribute : Attribute
    {
    }
}
