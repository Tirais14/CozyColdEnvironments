using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using ZLinq;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.UI
{
    public class SelectableObserver<T> : CCBehaviour, ISelectableObserver<T>
        where T : ISelectable
    {
        protected readonly ReactiveProperty<Maybe<T>> selection = new();
        protected readonly HashSet<T> selectables = new();

        public Maybe<T> Selection => selection.Value;

        protected virtual void OnTransformChildrenChanged()
        {
            CollectSelectables();
        }

        public IObservable<Unit> ObserveDeselected()
        {
            return selection.Where(x => x.IsNone).AsUnitObservable();
        }

        public IObservable<PreviousCurrentPair<Maybe<T>, T>> ObserveSelected()
        {
            return selection.Where(x => x.IsSome)
                            .Pairwise()
                            .Select(pair => PreviousCurrentPair.CreateT(pair.Previous, pair.Current.GetValueUnsafe()));
        }

        public IObservable<PreviousCurrentPair<Maybe<T>>> ObserveSelection()
        {
            return selection.Pairwise().Select(pair => PreviousCurrentPair.Create(pair.Previous, pair.Current));
        }

        protected virtual void CollectSelectables()
        {
            selectables.Clear();
            foreach (var cmp in this.QueryTo()
                                     .NotRecursive()
                                     .ChildrenGameObjects()
                                     .ZL()
                                     .Select(go => go.QueryTo().ByChildren().Model<T>().Lax())
                                     .Where(cmp => cmp.IsSome)
                                     .Select(cmp => cmp.GetValueUnsafe()))
            {
                selectables.Add(cmp);
            }
        }
    }
    public class SelectableObserver : SelectableObserver<ISelectable>
    {
    }
}
