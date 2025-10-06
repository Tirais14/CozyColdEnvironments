#nullable enable
using System;

namespace CCEnvs.Diagnostics
{
    [Flags]
    public enum DebugArguments
    {
        None,
        Default = None,
        /// <summary>
        /// Marks log message as additive. These messages can be turned off by separate setting
        /// </summary>
        IsAdditive = 1,
        /// <summary>
        /// Marks log message as editor only. These messages can be turned off by separate setting
        /// </summary>
        Editor = 2
    }
}
