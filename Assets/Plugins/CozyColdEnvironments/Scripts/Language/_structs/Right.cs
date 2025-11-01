using System.Runtime.CompilerServices;

namespace CCEnvs.FuncLanguage
{
    public readonly struct Right<T>
    {
        public T Value { get; }

        public Right(T value)
        {
            Value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(Right<T> left) => left.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Right<T>(T input) => new(input);
    }
}
