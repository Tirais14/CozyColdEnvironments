#nullable enable
using CCEnvs.Attributes;

namespace CCEnvs.Diagnostics
{
    public readonly struct DebugContext
    {
        public object? Target { get; }
        public DebugArguments Arguments { get; }

        public DebugContext(object? target = null, DebugArguments arguments = default)
        {
            Target = target;
            Arguments = arguments;
        }

        [CCConstructor]
        public DebugContext Additive(object? target = null)
        {
            return new DebugContext(target, DebugArguments.IsAdditive);
        }
    }
}
