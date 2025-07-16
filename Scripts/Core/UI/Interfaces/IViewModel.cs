using System;

#nullable enable
namespace UTIRLib.UI
{
    public interface IViewModel : IDisposable
    {
        void BindDisposable(IDisposable disposable);
    }
    public interface IViewModel<out T> : IViewModel
    {
        T Model { get; } 
    }
}
