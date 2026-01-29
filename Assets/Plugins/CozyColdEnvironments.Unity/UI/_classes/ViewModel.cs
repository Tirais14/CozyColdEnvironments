#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

#pragma warning disable S1699
namespace CCEnvs.Unity.UI
{
    public abstract class ViewModel<TModel> : IViewModel<TModel>, IDisposable
    {
        protected readonly CompositeDisposable disposables = new();
        private bool disposed;

        public TModel model { get; private set; }

        protected ViewModel(TModel model)
        {
            this.model = model;

            AddDisposableViewModelDataToList();
        }

        public void Dispose() => Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            disposables.DisposeEach();
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
