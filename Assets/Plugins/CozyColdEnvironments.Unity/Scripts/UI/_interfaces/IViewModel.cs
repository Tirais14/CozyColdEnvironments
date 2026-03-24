#nullable enable
#pragma warning disable IDE1006
using R3;
using System.Threading;

namespace CCEnvs.Unity.UI
{
    public interface IViewModel
    {
        object? Model { get; }

        void SetModel(object? model);

        CancellationToken DisposeCancellationToken { get; }

        bool HasModel();
        bool HasModel<T>();

        Observable<object?> ObserveModel();
    }
    public interface IViewModel<TModel> : IViewModel
    {
        new TModel? Model { get; }

        void SetModel(TModel model);

        new Observable<TModel?> ObserveModel();

        object IViewModel.Model => Model!;

        void IViewModel.SetModel(object? model) => SetModel(model.CastTo<TModel>());

        Observable<object?> IViewModel.ObserveModel() => ObserveModel().Cast<TModel?, object?>();
    }
}
