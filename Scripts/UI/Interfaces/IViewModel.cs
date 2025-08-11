using System;

#nullable enable
namespace UTIRLib.UI
{
    public interface IViewModel : IDisposable
    {
        object GetModel();
    }
    public interface IViewModel<out T> : IViewModel
    {
        new T GetModel();
    }
}
