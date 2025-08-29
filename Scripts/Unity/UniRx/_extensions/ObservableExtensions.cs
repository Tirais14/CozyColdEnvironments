using System;
using UnityEngine;
using CCEnvs.Diagnostics;
using UniRx;

#nullable enable
namespace CCEnvs.UniRx
{
    public static class ObservableExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static IObservable<bool> WhenTrue(this IObservable<bool> value)
        {
            if (value.IsNull())
                throw new ArgumentNullException("value");

            return value.Where(x => x);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static IObservable<bool> WhenFalse(this IObservable<bool> value)
        {
            if (value.IsNull())
                throw new ArgumentNullException("value");

            return value.Where(x => !x);
        }
    }
}
