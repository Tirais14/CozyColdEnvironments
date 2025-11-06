using System;

#nullable enable
namespace CCEnvs.Reflection
{
    [Flags]
    public enum TypeMatchingSettings
    {
        None,
        ByBaseGenericTypeDefinition,
        Default = None
    }
}
