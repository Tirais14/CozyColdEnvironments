#nullable enable
using CCEnvs.Disposables;
using R3;
using System;

namespace CCEnvs.UnityX.UI.Menus
{
    public abstract class ContextMenuItemViewModel<TModel> 
        :
        ViewModel<TModel>, 
        IContextMenuItemViewModel

        where TModel : IContextMenuItem
    {
        private readonly ReactiveProperty<string> name = new();

        private IDisposable? modelBinding;

        public ReadOnlyReactiveProperty<string> Name => name;

        public ContextMenuItemViewModel()
        {
            BindModel();
        }

        protected override void OnSetModel(TModel? model) { }

        protected override void InitModel(TModel model) { }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                CCDisposable.Dispose(ref modelBinding);
        }

        protected abstract string ConvertName(string rawName);

        private void OnModelChanged(TModel? model)
        {
            if (model is null)
                name.Value = string.Empty;
            else
                name.Value = ConvertName(model.Name);
        }

        private void BindModel()
        {
            modelBinding = ObserveModel().Subscribe(OnModelChanged);
        }
    }
}
