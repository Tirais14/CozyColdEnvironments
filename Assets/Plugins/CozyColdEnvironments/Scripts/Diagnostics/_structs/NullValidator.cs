#nullable enable

using System.Runtime.CompilerServices;

namespace CCEnvs.Diagnostics
{
    public struct NullValidator<T>
    {
        public bool IsNull { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get; 

            private set;
        }

        public NullValidator(T? obj)
        {
            if (obj is null)
                IsNull = true;
            else if (obj.Equals(null))
                IsNull = true;
            else
                IsNull = false;
        }
    }
}