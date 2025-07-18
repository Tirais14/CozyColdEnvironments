using System;
using System.Reflection;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Disposables
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

            FieldInfo field = DisposableContainerCache.GetCollectionField(GetType());

            var collection = (IDisposableCollection)field.GetValue(this);

            collection.Add(disposable);
        }
    }
}
