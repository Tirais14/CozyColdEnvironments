#nullable enable
using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Linq;
using System.Reflection;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.UI.MVVM
{
    public abstract class ViewModel<T> : DisposableContainer, IViewModel<T>
    {
        protected Reflected selfReflected { get; private set; }

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

            selfReflected = new Reflected(this);
        }

        public virtual void ForceNotify()
        {
            foreach (var (value, method) in 
                   from field in selfReflected.AllFields.Value
                   where field.FieldType.IsGenericType
                   where field.FieldType.Name.ContainsOrdinal("ReactiveProperty")
                   select (field, prop: field.GetValue(this)) into x
                   select (x.field, propRef: x.prop.AsReflected()) into x
                   select (value: x.propRef.Property(nameof(ReactiveProperty<object>.Value)).GetValue(),
                   method: x.field.FieldType.GetMethod(nameof(ReactiveProperty<object>.SetValueAndForceNotify), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new Type[] { x.field.GetValue(this).GetType() }, Array.Empty<ParameterModifier>())) into x
                   where x.method != null
                   select x)
            {
                method.Invoke(this, new object[] { value });
            }
        }
    }
}
