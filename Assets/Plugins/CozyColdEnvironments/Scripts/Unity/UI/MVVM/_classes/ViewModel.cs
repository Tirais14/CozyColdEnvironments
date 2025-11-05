#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.UI.MVVM
{
    public abstract class ViewModel<T> : IViewModel<T>, IDisposable
    {
        protected readonly List<IDisposable> disposables = new();
        private bool disposed;

        public Maybe<GameObject> gameObject { get; private set; }
        public T model { get; private set; }

        protected ViewModel(T model, GameObject gameObject)
        {
            this.model = model;
            this.gameObject = gameObject;

            new Maybe<T>(model).Map(x => x as IGameObjectBindable)
                .IfSome(target => target.Reflect()
                                        .NonPublic()
                                        .ExtraType<GameObject>()
                                        .Field()
                                        .Strict()
                                        .SetValue(target, gameObject)
                                        );
        }

        public virtual void ForceNotify()
        {
            foreach (var (value, method) in 
                   from field in this.Reflect().NonPublic().Fields()
                   where field.FieldType.IsGenericType
                   where field.FieldType.Name.ContainsOrdinal("ReactiveProperty")
                   select (field, prop: field.GetValue(this)) into x
                   select (value: x.Reflect()
                       .NonPublic()
                       .Name(nameof(ReactiveProperty<object>.Value))
                       .Property()
                       .Lax()
                       .Map(f => f.GetValue(this)).Raw,
                   method: x.Reflect()
                       .Name(nameof(ReactiveProperty<object>.SetValueAndForceNotify))
                       .Method()
                       .Strict()) into x
                   where x.method != null
                   select x)
            {
                method.Invoke(this, new object[] { value });
            }
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
