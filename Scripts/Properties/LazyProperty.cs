using System;
using System.Collections;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib
{
    public class LazyProperty<T> : IProperty<T>
    {
        private readonly Func<T> initFunc;

        private bool isInited;
        private T? value;

        public T Value {
            get
            {
                if (!isInited)
                {
                    T? previousValue = value;

                    value = initFunc();

                    if (Comparer.Default.Compare(previousValue, value) == 0)
                        throw new TirLibException("Value wasn't be changed.");

                    isInited = true;
                }



                return value!;
            }
        }

        public LazyProperty(Func<T> initFunc)
        {
            this.initFunc = initFunc;
        }

        public static implicit operator T(LazyProperty<T> prop) => prop.Value;
    }
}
