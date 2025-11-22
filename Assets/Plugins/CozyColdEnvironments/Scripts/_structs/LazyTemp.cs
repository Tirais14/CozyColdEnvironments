using CCEnvs.FuncLanguage;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    public readonly struct LazyTemp<T>
    {
        private readonly Maybe<T> value;
        private readonly Func<T> factory;

        public readonly T Value => Getvalue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T Getvalue()
        {
            if (value.IsSome)
                return value.GetValueUnsafe();
            return 
                factory();
        }
    }
}
