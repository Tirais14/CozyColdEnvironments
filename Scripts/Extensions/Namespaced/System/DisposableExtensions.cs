#nullable enable
using System;
using UTIRLib.Diagnostics;

namespace UTIRLib.Disposables
{
    public static class DisposableExtensions
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
        public static IDisposable AddTo(this IDisposable value,
                                        IDisposableCollection collection)
        {
            return value.AddTo<IDisposable>(collection);
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
