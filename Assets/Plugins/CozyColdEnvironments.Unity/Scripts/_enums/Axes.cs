using System;

#nullable enable
namespace CCEnvs.Unity
{
    [Flags]
    public enum Axes
    {
        None,
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
        All = X | Y| Z
    }
}
