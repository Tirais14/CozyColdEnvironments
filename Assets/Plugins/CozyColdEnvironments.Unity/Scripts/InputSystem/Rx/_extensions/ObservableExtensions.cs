using R3;
using System;
using static UnityEngine.InputSystem.InputAction;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public static class ObservableExtensions
    {


        /// <exception cref="ArgumentNullException"></exception>
        public static Observable<CallbackContext> WhenStarted(
            this Observable<CallbackContext> value)
        {
            if (value.IsNull())
                throw new ArgumentNullException("value");

            return value.Where(x => x.started);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static Observable<CallbackContext> WhenPerformed(
            this Observable<CallbackContext> value)
        {
            if (value.IsNull())
                throw new ArgumentNullException("value");

            return value.Where(x => x.performed);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static Observable<CallbackContext> WhenCanceled(
            this Observable<CallbackContext> value)
        {
            if (value.IsNull())
                throw new ArgumentNullException("value");

            return value.Where(x => x.canceled);
        }
    }
}
