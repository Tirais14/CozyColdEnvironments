#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI
{
    public interface IViewModel
    {
        object model { get; }

        //void ForceNotify();
    }
    public interface IViewModel<TModel> : IViewModel
    {
        new TModel model { get; }

        object IViewModel.model => model!;
    }
}
