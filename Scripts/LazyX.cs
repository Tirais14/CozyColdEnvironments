#nullable enable
using System.Threading;

namespace UTIRLib
{
    public class LazyX<T> : System.Lazy<T>
    {
        public LazyX()
        {
        }

        public LazyX(bool isThreadSafe) : base(isThreadSafe)
        {
        }

        public LazyX(System.Func<T> valueFactory) : base(valueFactory)
        {
        }

        public LazyX(LazyThreadSafetyMode mode) : base(mode)
        {
        }

        public LazyX(T value) : base(value)
        {
        }

        public LazyX(System.Func<T> valueFactory,
                    bool isThreadSafe) 
            : base(valueFactory,
                   isThreadSafe)
        {
        }

        public LazyX(System.Func<T> valueFactory,
                    LazyThreadSafetyMode mode) :
            base(valueFactory,
                 mode)
        {
        }

        public static implicit operator T(LazyX<T> prop) => prop.Value;
    }
}
