using CCEnvs.Language;
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
        private readonly ReactiveProperty<KeyValuePair<TKey, Ghost<TValue>>> selection = new();
        private readonly Trapped<Func<TKey, TValue>> valueGetter;
        private bool disposed;

        public event Action<SelectionChangedEvent<TKey, TValue>>? OnSelectionChanged;

        public IReadOnlyReactiveProperty<KeyValuePair<TKey, Ghost<TValue>>> Selection => selection;

        public SelectionController(Func<TKey, TValue> valueGetter)
        {
            this.valueGetter = valueGetter;
        }

        public void SelectIt(TKey key)
        {
            if (EqualityComparer<TKey>.Default.Equals(selection.Value.Key, key))
                return;

            var info = SelectionChangedEvent.Create(
                selection.Value.Key,
                selection.Value.Value.Value(),
                key,
                valueGetter.ValueUnsafe().Invoke(key));

            info.previousValue.AsOrDefault<ISelectable>().IfSome(x => x.SelectIt());
            selection.Value = info.NewSelection;

            selectionSubj.Value.OnNext(info);
            OnSelectionChanged?.Invoke(info);
        }

        public void DeselectIt(TKey key)
        {
            if (Selection.IsDefault())
                return;

            var info = SelectionChangedEvent.Create(
                selection.Value.Key,
                selection.Value.Value.Value(),
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
                DeselectIt(key);
                return;
            }

            SelectIt(key);
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
