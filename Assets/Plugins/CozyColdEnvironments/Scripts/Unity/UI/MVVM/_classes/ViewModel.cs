#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using UniRx;
using UnityEngine;
using System.Linq;

#pragma warning disable S1699
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

            AddDisposableViewModelDataToList();
        }

        //public virtual void ForceNotify()
        //{
        //    foreach (var rxProp in this.Reflect()
        //                           .NonPublic()
        //                           .TypeFilter(typeof(IReactiveProperty<>))
        //                           .MatchTypesByBaseGenericTypeDefinition()
        //                           .Cache()
        //                           .Fields()
        //                           .Select(field => field.GetValue(this)))
        //    {
        //        PropertyInfo? valueProp = rxProp.Reflect()
        //                                        .Name(nameof(ReactiveProperty<object>.Value))
        //                                        .Property()
        //                                        .Strict();

        //        object propValue = valueProp.GetValue(rxProp);

        //        if (propValue.IsNull())
        //        {
        //            rxProp.Reflect()
        //                  .Name(nameof(ReactiveProperty<object>.SetValueAndForceNotify))
        //                  .ArgumentTypes(valueProp.PropertyType)
        //                  .InvokeMethod();
        //        }
        //        else
        //        {
        //            rxProp.Reflect()
        //                  .Name(nameof(ReactiveProperty<object>.SetValueAndForceNotify))
        //                  .Arguments(propValue)
        //                  .InvokeMethod();
        //        }
        //    }
        //}

        public void Dispose() => Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            disposables.DisposeAll();
            disposables.Clear();

            disposed = true;
        }

        protected virtual void AddDisposableViewModelDataToList()
        {
            foreach (var item in this.Reflect().Cache().GetFieldValues<IDisposable>())
                disposables.Add(item);
        }
    }
}
