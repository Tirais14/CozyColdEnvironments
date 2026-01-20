using System;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    [Flags]
    public enum CommandStatus
    {
        None,
        Running,
        Completed = 1 << 1,
        Canceled = 1 << 2,
        Faulted = 1 << 3
    }
}
