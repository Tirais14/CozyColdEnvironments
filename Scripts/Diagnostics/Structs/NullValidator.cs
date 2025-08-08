#nullable enable

namespace UTIRLib.Diagnostics
{
    public readonly struct NullValidator<T>
    {
        public readonly bool isNull;

        public readonly bool AnyNull => isNull;

        public NullValidator(T? obj)
        {
            if (obj is null)
                isNull = true;
            else if (obj.Equals(null))
                isNull = true;
            else
                isNull = false;
        }
    }
}