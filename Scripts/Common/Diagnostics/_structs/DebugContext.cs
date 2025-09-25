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

        /// <summary>
        /// It's mock, for short return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>()
        {
            return default!;
        }

        public readonly void Deconstruct(out object? target, out DebugArguments arguments)
        {
            target = Target;
            arguments = Arguments;
        }
    }
}
