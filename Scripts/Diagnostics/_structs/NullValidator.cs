#nullable enable

namespace CozyColdEnvironments.Diagnostics
{
    public struct NullValidator<T>
    {
        public bool IsNull { readonly get; private set; }

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