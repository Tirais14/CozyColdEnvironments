#nullable enable
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

        public static DebugContext Additive(object? target = null)
        {
            return new DebugContext(target, DebugArguments.IsAdditive);
        }

        public void Deconstruct(out object? target, out DebugArguments arguments)
        {
            target = Target;
            arguments = Arguments;
        }
    }
}
