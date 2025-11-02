#nullable enable
using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using UnityEngine;

namespace CCEnvs.Unity.UI.MVVM
{
    public abstract class ViewModel<T> : DisposableContainer, IViewModel<T>
    {
        public Maybe<GameObject> gameObject { get; private set; }
        public T model { get; private set; }

        protected ViewModel(T model, GameObject gameObject)
        {
            this.model = model;
            this.gameObject = gameObject;

            new Maybe<T>(model).Map(x => x as IGameObjectBindable)
                .IfSome(x =>
                {
                    var xR = x!.AsReflected();
                    try
                    {
                        xR.Property(nameof(gameObject)).SetValue(new Maybe<GameObject?>(gameObject));
                    }
                    catch (System.Exception)
                    {
                        xR.Field("_gameObject").SetValue(new Maybe<GameObject?>(gameObject));
                    }
                });
        }
    }
}
