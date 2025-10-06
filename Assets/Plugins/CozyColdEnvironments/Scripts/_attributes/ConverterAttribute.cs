#nullable enable
using System;

namespace CCEnvs.Attributes
{
    /// <summary>
    /// For reflection convertings
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,
        AllowMultiple = false,
        Inherited = true)]
    public class ConverterAttribute : Attribute
    {
    }
}
