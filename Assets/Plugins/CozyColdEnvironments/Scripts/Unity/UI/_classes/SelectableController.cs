using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.UI.MVVM;
using System.Collections.Generic;
using System.Linq;
using UniRx;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.UI
{
    public abstract class SelectableController<TKey, TView, TValue> 
        : CCBehaviour,
        ISelectableController<TKey, TValue>

        where TView : UnityEngine.Object, IView, ISelectable
    {
        protected readonly IEqualityComparer<TKey> keyComparer = EqualityComparer<TKey>.Default;
        protected readonly HashSet<TView> views;
        protected readonly ReactiveProperty<Maybe<KeyValuePair<TKey, TValue>>> selection = new();

        public IReadOnlyReactiveProperty<Maybe<KeyValuePair<TKey, TValue>>> Selection => selection;

        protected virtual void OnTransformChildrenChanged()
        {
            foreach (var item in this.QueryTo()
                                     .ByChildren()
                                     .NotRecursive()
                                     .Components<TView>()
                                     .Where(view => view.model is TValue))
            {
                views.Add(item);
            }
        }

        protected virtual void OnDestroy()
        {
            selection.Dispose();
        }

        public abstract void DoSelect(TKey key);

        public abstract void DoDeselect(TKey key);

        public void SwitchSelectionState(TKey key)
        {
            if (keyComparer.Equals(key, selection.Value.Raw.Key))
            {
                DoDeselect(key);
                return;
            }

            DoSelect(key);
        }
    }
}
