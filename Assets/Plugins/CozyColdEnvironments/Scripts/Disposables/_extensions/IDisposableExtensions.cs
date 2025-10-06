using CCEnvs.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Disposables
{
    public static class IDisposableExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static T AddTo<T>(this T value,
                                 IDisposableCollection collection)
            where T : IDisposable
        {
            if (collection.IsNull())
                throw new ArgumentNullException(nameof(collection));

            collection.Add(value);

            return value;
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static T AddTo<T>(this T value,
                                 IDisposableContainer container)
            where T : IDisposable
        {
            if (container.IsNull())
                throw new ArgumentNullException(nameof(container));

            container.Add(value);

            return value;
        }
        public static IDisposable AddTo(this IDisposable value,
                                        IDisposableContainer container)
        {
            return value.AddTo<IDisposable>(container);
        }
    }
}
