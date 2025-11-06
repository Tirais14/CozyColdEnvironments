#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.UI.MVVM
{
    public abstract class ViewModel<TModel> : IViewModel<TModel>, IDisposable
    {
        protected readonly List<IDisposable> disposables = new();
        private bool disposed;

        public Maybe<GameObject> gameObject { get; private set; }
        public TModel model { get; private set; }
        public virtual bool ModelMutable => false;

        protected ViewModel(TModel model, GameObject gameObject)
        {
            this.model = model;
            this.gameObject = gameObject;

            model.Maybe()
                .Map(x => x as IGameObjectBindable)
                .Where(x => x is not UnityEngine.Object)
                .IfSome(target => target.Reflect()
                    .NonPublic()
                    .Name(nameof(IGameObjectBindable.gameObject))
                    .Arguments(gameObject.Maybe())
                    .SetFieldValue()
                    );
        }

        public virtual void ForceNotify()
        {
            foreach (var rxProp in
                   from field in this.Reflect()
                           .NonPublic()
                           .Name(nameof(ReactiveProperty<object>))
                           .ExtraType(typeof(IReactiveProperty<>))
                           .MatchTypesByBaseGenericTypeDefinition()
                           .Cache()
                           .Fields()
                   select field.GetValue(this))
            {
                object propValue = rxProp.Reflect()
                                         .Name(nameof(ReactiveProperty<object>.Value))
                                         .GetPropertyValue();

                rxProp.Reflect()
                      .Name(nameof(ReactiveProperty<object>.SetValueAndForceNotify))
                      .Arguments(propValue)
                      .InvokeMethod();
            }
        }

        public virtual void SetModelUnsafe(TModel model)
        {
            if (!ModelMutable)
                throw new InvalidOperationException("Cannot set new model.");
            CC.Guard.IsNotNull(model, nameof(model));

            this.model = model;
            ForceNotify();
        }

        public void Dispose() => Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            disposables.DisposeAll();

            disposed = true;
        }
    }
}
