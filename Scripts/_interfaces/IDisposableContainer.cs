using System;
using System.Reflection;
using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Disposables
{
    /// <summary>
    /// Inherits from this, for auto implementing <see cref="DisposableExtensions.AddTo(IDisposable, IDisposableContainer)"/>
    /// </summary>
    public interface IDisposableContainer : IDisposable
    {
        /// <exception cref="ArgumentNullException"></exception>
        void Add(IDisposable disposable)
        {
            if (disposable.IsNull())
                throw new ArgumentNullException(nameof(disposable));

            Type reflectedType = GetType();
            FieldInfo field = DisposableContainerCache.GetCollectionField(reflectedType);

            var collection = (IDisposableCollection)field.GetValue(this);

            collection.Add(disposable);
        }
    }
}
