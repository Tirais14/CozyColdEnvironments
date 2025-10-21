#nullable enable
namespace CCEnvs.Unity.UI.MVVM
{
    public interface IViewModel : IGameObjectBindable
    {
        object GetModel();
    }
    public interface IViewModel<out T> : IViewModel
    {
        new T GetModel();
    }
}
