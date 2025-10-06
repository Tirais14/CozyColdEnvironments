using System;

#nullable enable
namespace CCEnvs
{
    [Flags]
    public enum InjectionOptions
    {
        None,
        ThrowIfFailed,
        CacheMember = 2,
        Default = ThrowIfFailed
    }
}
