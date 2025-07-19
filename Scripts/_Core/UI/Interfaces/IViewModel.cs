using System;
using UTIRLib.Disposables;

#nullable enable
namespace UTIRLib.UI
{
    public interface IViewModel : IDisposable
    {
    }
    public interface IViewModel<out T> : IViewModel
    {
        T Model { get; } 
    }
}
