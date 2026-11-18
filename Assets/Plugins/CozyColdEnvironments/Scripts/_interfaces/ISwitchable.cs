#nullable enable

using R3;
using System.Collections.Generic;

namespace CCEnvs
{
    public interface ISwitchable
    {
        bool IsEnabled { get; }

        void Enable();

        void Disable();

        Observable<bool> ObserveEnabled();

        Observable<bool> ObserveDisabled();
    }

    public static class ISwitchableExtensions
    {
        public static void DisableAll<T>(this IEnumerable<T> collection)
            where T : ISwitchable
        {
            CC.Guard.IsNotNull(collection, nameof(collection));

            foreach (var item in collection)
                item.Disable();
        }

        public static void EnableAll<T>(this IEnumerable<T> collection)
            where T : ISwitchable
        {
            CC.Guard.IsNotNull(collection, nameof(collection));

            foreach (var item in collection)
                item.Enable();
        }
    }
}