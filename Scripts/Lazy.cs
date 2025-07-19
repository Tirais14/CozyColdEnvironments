#nullable enable
using System.Threading;

namespace UTIRLib
{
    public class Lazy<T> : System.Lazy<T>
    {
        public Lazy()
        {
        }

        public Lazy(bool isThreadSafe) : base(isThreadSafe)
        {
        }

        public Lazy(System.Func<T> valueFactory) : base(valueFactory)
        {
        }

        public Lazy(LazyThreadSafetyMode mode) : base(mode)
        {
        }

        public Lazy(T value) : base(value)
        {
        }

        public Lazy(System.Func<T> valueFactory,
                    bool isThreadSafe) 
            : base(valueFactory,
                   isThreadSafe)
        {
        }

        public Lazy(System.Func<T> valueFactory,
                    LazyThreadSafetyMode mode) :
            base(valueFactory,
                 mode)
        {
        }

        public static implicit operator T(Lazy<T> prop) => prop.Value;
    }
}
