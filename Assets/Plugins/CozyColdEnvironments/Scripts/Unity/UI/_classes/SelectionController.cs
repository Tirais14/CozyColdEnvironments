using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;
using UniRx;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.UI
{
    public class SelectionController<TKey, TValue> 
        : ISelectionController<TKey, TValue>,
        IDisposable
    {
        private readonly ReactiveProperty<(TKey key, Maybe<TValue> value)> selection = new();
        private readonly Catched<Func<TKey, TValue>> valueGetter;
        private bool disposed;

        public IReadOnlyReactiveProperty<(TKey key, Maybe<TValue> it)> Selection => selection;

        public SelectionController(Func<TKey, TValue> valueGetter)
        {
            this.valueGetter = valueGetter;
        }

        public void DoSelect(TKey key)
        {
            selection.Value = (key, valueGetter.Map(x => x(key)).AccessUnsafe());

            this.PrintLog($"Selected by key: {key}.");
        }

        public void DoDeselect(TKey key)
        {
            selection.Value.value.IfSome(x => this.PrintLog($"Deselected by key: {key}"));
            selection.Value = default;
        }

        public void SwitchSelectionState(TKey key)
        {
            if (EqualityComparer<TKey>.Default.Equals(selection.Value.key, key))
            {
                DoDeselect(key);
                return;
            }

            DoSelect(key);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            selection.Dispose();

            disposed = true;
        }
    }
}
