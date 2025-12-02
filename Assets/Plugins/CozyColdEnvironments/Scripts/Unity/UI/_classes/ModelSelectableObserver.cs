using CCEnvs.FuncLanguage;
using CCEnvs.Unity.UI;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity
{
    public class ModelSelectableObserver<TModel> : SelectableObserver<ISelectable>, ISelectableController<TModel>
    {
        protected readonly ReactiveProperty<Maybe<TModel>> modelSelection = new();

        Maybe<TModel> ISelectableController<TModel>.Selection => modelSelection.Value;

        protected override void Start()
        {
            base.Start();

            selection.SubscribeWithState(this,
                static (sel, @this) =>
                {
                    @this.modelSelection.Value = sel.Raw.As<Component>()
                        .Match(some: cmp => cmp.QueryTo().Model<TModel>().Lax().GetValue(),
                               none: () => sel.As<TModel>().Raw
                               );
                }).AddTo(this);
        }

        IObservable<TModel> ISelectableController<TModel>.ObserveDeselected()
        {
            return modelSelection.Pairwise()
                                 .Where(pair => pair.Current.IsNone && pair.Previous.IsSome)
                                 .Select(pair => pair.Previous.GetValueUnsafe());
        }

        IObservable<TModel> ISelectableController<TModel>.ObserveSelected()
        {
            return modelSelection.Where(x => x.IsSome).Select(x => x.GetValueUnsafe());
        }

        IObservable<PreviousCurrentPair<Maybe<TModel>>> ISelectableController<TModel>.ObserveSelection()
        {
            return modelSelection.Pairwise().Select(pair => PreviousCurrentPair.Create(pair.Previous, pair.Current));
        }
    }
}
