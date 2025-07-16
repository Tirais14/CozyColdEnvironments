#nullable enable
namespace UTIRLib.UI
{
    public interface IView
    {
        
    }
    public interface IView<out T> : IView
    {
        T ViewModel { get; }
    }
}
