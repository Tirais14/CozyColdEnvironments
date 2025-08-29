using System;
using UniRx;
using CCEnvs.Diagnostics;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace CCEnvs.InputSystem.Reactive
{
    public static class ObservableExtensions
    {


        /// <exception cref="ArgumentNullException"></exception>
        public static IObservable<CallbackContext> WhenStarted(
            this IObservable<CallbackContext> value)
        {
            if (value.IsNull())
                throw new ArgumentNullException("value");

            return value.Where(x => x.started);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static IObservable<CallbackContext> WhenPerformed(
            this IObservable<CallbackContext> value)
        {
            if (value.IsNull())
                throw new ArgumentNullException("value");

            return value.Where(x => x.performed);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static IObservable<CallbackContext> WhenCanceled(
            this IObservable<CallbackContext> value)
        {
            if (value.IsNull())
                throw new ArgumentNullException("value");

            return value.Where(x => x.canceled);
        }
    }
}
