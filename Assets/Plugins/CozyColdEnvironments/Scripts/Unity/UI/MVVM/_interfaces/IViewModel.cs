#nullable enable
#pragma warning disable IDE1006
using System.Runtime.CompilerServices;

namespace CCEnvs.Unity.UI.MVVM
{
    public interface IViewModel : IGameObjectBindable
    {
        object model { get; }
        bool ModelMutable { get; }

        void SetModelUnsafe(object model);

        void ForceNotify();
    }
    public interface IViewModel<TModel> : IViewModel
    {
        new TModel model { get; }

        object IViewModel.model => model!;

        void SetModelUnsafe(TModel model);

        void IViewModel.SetModelUnsafe(object model) => SetModelUnsafe(model.As<TModel>());
    }
}
