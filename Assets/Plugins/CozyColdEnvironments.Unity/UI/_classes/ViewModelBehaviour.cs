using CCEnvs.Collections;
using CCEnvs.Unity.Components;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public abstract class ViewModelBehaviour<TModel> : CCBehaviour,
        IViewModel<TModel>,
        IDisposable
    {
        protected readonly List<IDisposable> modelDisposables = new();

        private readonly ReactiveProperty<TModel> m_Model = new();

        public TModel model => m_Model.Value;

        public CancellationToken DisposeCancellationToken => destroyCancellationToken;

        protected override void Awake()
        {
            base.Awake();
            BindModel();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_Model.Dispose();
        }

        public ViewModelBehaviour<TModel> SetModelFluentUntyped(TModel model)
        {
            CC.Guard.IsNotNull(model, nameof(model));

            m_Model.Value = model;

            return this;
        }

        protected virtual void Init()
        {

        }

        private void BindModel()
        {
            m_Model.Subscribe(this,
                static (model, @this) =>
                {
                    if (model.IsNull())
                        return;

                    @this.modelDisposables.DisposeEachAndClear();
                    @this.Init();
                })
                .RegisterDisposableTo(this);
        }

        private bool disposed;
        public void Dispose()
        {
        }
        protected virtual void Dispose(bool state)
        {
            if (disposed)
                return;

            if (state)
                Destroy(this);

            disposed = true;
        }
    }

    public static class ViewModelBehaviourExtensions
    {
        public static TViewModel SetModel<TViewModel, TModel>(
            this TViewModel source,
            TModel model
            )
            where TViewModel : ViewModelBehaviour<TModel>
        {
            CC.Guard.IsNotNullSource(source);

            return (TViewModel)source.SetModelFluentUntyped(model);
        }
    }
}
