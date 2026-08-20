using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Proeprties
{
    public class Trigger<T>
    {
        private readonly T? @default;
        private T? value;

        public Trigger()
        {
        }

        public Trigger(T value, T? @default = default)
        {
            this.value = value;
            this.@default = @default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? PeekValue() => value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? GetValue()
        {
            var t = value;
            value = @default;
            return t;
        }

        public bool TryGetValue([NotNullWhen(true)] out T? result)
        {
            if (value.IsNull())
            {
                result = default;
                return false;
            }

            result = GetValue()!;
            return true;
        }

        public void SetValue(T value)
        {
            this.value = value;
        }
    }
}
