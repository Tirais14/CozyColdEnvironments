#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI.MVVM
{
    public interface IViewModel
    {
        object model { get; }

        //void ForceNotify();
    }
    public interface IViewModel<out TModel> : IViewModel
    {
        new TModel model { get; }

        object IViewModel.model => model!;
    }
}
