using System;

#nullable enable
namespace CCEnvs.UnityX.Services
{
    [Flags]
    public enum ServiceBindActionOptions
    {
        None,
        WithInterfaces = 1 << 0,
        WithBaseTypes = 1 << 1
    }
}
