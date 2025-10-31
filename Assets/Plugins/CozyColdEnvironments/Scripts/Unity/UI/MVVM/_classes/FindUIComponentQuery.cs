using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.MVVM
{
    public class FindUIComponentQuery : FindComponentQuery
    {
        new public readonly static FindUIComponentQuery Instance = new();

        new public static FindUIComponentQuery Empty => new();

        public IEnumerable<IView> Views(Type? type = null)
        {
            type ??= typeof(IView);

            return Components(type).Cast<IView>();    
        }

        public IEnumerable<T> Views<T>()
            where T : IView
        {
            return Views(typeof(T)).Cast<T>();
        }

        public Maybe<IView> View(Type? type = null)
        {
            return Views(type).FirstOrDefault().Maybe();
        }

        public Maybe<T> View<T>()
            where T : IView
        {
            return Views<T>().FirstOrDefault();
        }

        public IView ViewStrict(Type? type = null)
        {
            return View(type).IfNone(() => throw new ComponentNotFoundException(type, Source)).Target!;
        }

        public T ViewStrict<T>()
            where T : IView
        {
            return ViewStrict(typeof(T)).As<T>();
        }

        public IEnumerable<IViewModel> ViewModels(Type? type = null)
        {
            type ??= typeof(IViewModel);

            bool anyType = type is null;

            return from view in Views(type)
                   select view.viewModel into viewModel
                   where anyType || viewModel.IsType(type!)
                   select viewModel;  
        }

        public IEnumerable<T> ViewModels<T>()
            where T : IViewModel
        {
            return ViewModels(typeof(T)).Cast<T>();
        }

        public Maybe<IViewModel> ViewModel(Type? type = null)
        {
            return ViewModels(type).FirstOrDefault().Maybe();
        }

        public Maybe<T> ViewModel<T>()
            where T : IViewModel
        {
            return ViewModels<T>().FirstOrDefault();
        }

        public IViewModel ViewModelStrict(Type? type = null)
        {
            return ViewModel(type).IfNone(() => throw new ComponentNotFoundException(type, Source)).Target!;
        }

        public T ViewModelStrict<T>()
            where T : IViewModel
        {
            return ViewModelStrict(typeof(T)).As<T>();
        }

        public IEnumerable<object> Models(Type? type = null)
        {
            type ??= typeof(object);

            bool anyType = type is null;

            return from obj in Components(type)
                   select (obj, type: obj.GetType()) into x
                   select (x.obj, x.type, view: x.AsOrDefault<IView>()) into x
                   select x.view.Map(y => y.viewModel.model).Access(x.obj) into obj
                   where anyType || obj.IsType(type!)
                   select obj;
        }

        public IEnumerable<T> Models<T>()
        {
            return Models(typeof(T)).Cast<T>();
        }

        public Maybe<object> Model(Type? type = null)
        {
            return Models(type).FirstOrDefault();
        }

        public Maybe<T> Model<T>()
        {
            return Models<T>().FirstOrDefault();
        }

        public object ModelStrict(Type? type = null)
        {
            return Model(type).IfNone(() => throw new ComponentNotFoundException(type, Source)).Target!;
        }

        public T ModelStrict<T>()
        {
            return ModelStrict().As<T>();
        }
    }

    public static class FindUIComponentQueryExtensions
    {
        public static FindUIComponentQuery FindUIComponent(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return FindUIComponentQuery.Instance.From(source).As<FindUIComponentQuery>();
        }

        public static FindUIComponentQuery FindUIComponent(this Component source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return FindUIComponentQuery.Instance.From(source).As<FindUIComponentQuery>();
        }
    }
}
