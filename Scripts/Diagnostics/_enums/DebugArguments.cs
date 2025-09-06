#nullable enable
using System;

namespace CCEnvs.Diagnostics
{
    [Flags]
    public enum DebugArguments
    {
        None,
        /// <summary>
        /// Marks log message as additive. These messages can be turned off by separate setting
        /// </summary>
        IsAdditive
    }
}
