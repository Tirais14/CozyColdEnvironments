#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI.MVVM
{
    public interface IViewModel : IGameObjectBindable
    {
        object model { get; }
    }
    public interface IViewModel<out T> : IViewModel
    {
        new T model { get; }

        object IViewModel.model => model!;
    }
}
