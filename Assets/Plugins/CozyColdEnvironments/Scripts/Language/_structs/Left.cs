#nullable enable
using System.Runtime.CompilerServices;

namespace CCEnvs.FuncLanguage
{
    public readonly struct Left<T>
    {
        public T Value { get; }

        public Left(T value)
        {
            Value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(Left<T> left) => left.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Left<T>(T input) => new(input);
    }
}
