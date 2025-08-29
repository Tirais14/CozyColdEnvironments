using System;
using UnityEngine;
using UTIRLib.Diagnostics;
using UniRx;

#nullable enable
namespace UTIRLib.UniRx
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
