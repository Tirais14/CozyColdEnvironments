using CCEnvs.Collections;
using CCEnvs.Unity.Components;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public abstract class ViewModelBehaviour
        :
        CCBehaviour, 
        IViewModel,
        IDisposable
    {
        protected readonly ReactiveProperty<object> m_Model = new();

        public object model => m_Model.Value;

        public virtual CancellationToken DisposeCancellationToken => destroyCancellationToken;

        public ViewModelBehaviour SetModelFluentUntyped(object model)
        {
            CC.Guard.IsNotNull(model, nameof(model));

            m_Model.Value = model;

            return this;
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

        protected virtual void Init()
        {
        }
    }

    public abstract class ViewModelBehaviour<TModel> : ViewModelBehaviour,
        IViewModel<TModel>
    {
        protected readonly List<IDisposable> modelDisposables = new();

        new public TModel model => (TModel)m_Model.Value;

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

        protected override void Init()
        {
            base.Init();
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
                .AddDisposableTo(this);
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

        public static TViewModel SetModel<TViewModel>(
            this TViewModel source,
            object model
            )
            where TViewModel : ViewModelBehaviour
        {
            CC.Guard.IsNotNullSource(source);

            return (TViewModel)source.SetModelFluentUntyped(model);
        }
    }
}
