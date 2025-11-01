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
        private readonly Lazy<Subject<SelectionChangedEvent<TKey, TValue>>> selectionSubj = new(() => new Subject<SelectionChangedEvent<TKey, TValue>>());
        private readonly ReactiveProperty<KeyValuePair<TKey, Maybe<TValue>>> selection = new();
        private readonly Catched<Func<TKey, TValue>> valueGetter;
        private bool disposed;

        public event Action<SelectionChangedEvent<TKey, TValue>>? OnSelectionChanged;

        public IReadOnlyReactiveProperty<KeyValuePair<TKey, Maybe<TValue>>> Selection => selection;

        public SelectionController(Func<TKey, TValue> valueGetter)
        {
            this.valueGetter = valueGetter;
        }

        public void SelectItem(TKey key)
        {
            if (EqualityComparer<TKey>.Default.Equals(selection.Value.Key, key))
                return;

            var info = SelectionChangedEvent.Create(
                selection.Value.Key,
                selection.Value.Value.Access(),
                key,
                valueGetter.AccessUnsafe().Invoke(key));

            info.previousValue.AsOrDefault<ISelectable>().IfSome(x => x.SelectIt());
            selection.Value = info.NewSelection;

            selectionSubj.Value.OnNext(info);
            OnSelectionChanged?.Invoke(info);
        }

        public void DeselectItem(TKey key)
        {
            if (Selection.IsDefault())
                return;

            var info = SelectionChangedEvent.Create(
                selection.Value.Key,
                selection.Value.Value.Access(),
                default,
                default);

            info.previousValue.AsOrDefault<ISelectable>().IfSome(x => x.DeselectIt());
            selection.Value = info.NewSelection;

            selectionSubj.Value.OnNext(info!);
            OnSelectionChanged?.Invoke(info!);
        }

        public void SwitchSelectionState(TKey key)
        {
            if (EqualityComparer<TKey>.Default.Equals(selection.Value.Key, key))
            {
                DeselectItem(key);
                return;
            }

            SelectItem(key);
        }

        public IObservable<SelectionChangedEvent<TKey, TValue>> ObserveSelection()
        {
            return selectionSubj.Value;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            selectionSubj.Value.Dispose();

            disposed = true;
        }
    }
}
