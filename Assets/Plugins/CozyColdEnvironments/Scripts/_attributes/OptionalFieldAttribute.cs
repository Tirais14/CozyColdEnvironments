#nullable enable

using System;

namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true)]
    public class OptionalFieldAttribute : OptionalAttribute
    {
    }
}