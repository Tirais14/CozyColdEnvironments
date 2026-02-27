#nullable enable
using System;
using System.Threading;

namespace CCEnvs
{
    [Obsolete("Use Lazy instead")]
    /// <summary>
    /// Same as <see cref="System.Lazy{T}"/>, but implements implicit conversation operator
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
