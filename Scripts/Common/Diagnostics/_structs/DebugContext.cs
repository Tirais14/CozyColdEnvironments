#nullable enable
namespace CCEnvs.Diagnostics
{
    public struct DebugContext
    {
        public object? Target { get; }
        public DebugArguments Arguments { get; set; }

        public DebugContext(object? target = null, DebugArguments arguments = default)
        {
            Target = target;
            Arguments = arguments;
        }

        public DebugContext Additive()
        {
            Arguments |= DebugArguments.IsAdditive;

            return this;
        }

        public DebugContext Editor()
        {
            Arguments |= DebugArguments.Editor;

            return this;
        }

        public readonly void Deconstruct(out object? target, out DebugArguments arguments)
        {
            target = Target;
            arguments = Arguments;
        }
    }
}
