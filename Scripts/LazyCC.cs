#nullable enable
using System.Threading;

namespace CCEnvs
{
    public class LazyCC<T> : System.Lazy<T>
    {
        public LazyCC()
        {
        }

        public LazyCC(bool isThreadSafe) : base(isThreadSafe)
        {
        }

        public LazyCC(System.Func<T> valueFactory) : base(valueFactory)
        {
        }

        public LazyCC(LazyThreadSafetyMode mode) : base(mode)
        {
        }

        public LazyCC(T value) : base(value)
        {
        }

        public LazyCC(System.Func<T> valueFactory,
                    bool isThreadSafe) 
            : base(valueFactory,
                   isThreadSafe)
        {
        }

        public LazyCC(System.Func<T> valueFactory,
                    LazyThreadSafetyMode mode) :
            base(valueFactory,
                 mode)
        {
        }

        public static implicit operator T(LazyCC<T> prop) => prop.Value;
    }
}
