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
                    @this.modelSelection.Value = sel.Raw.AsOrDefault<Component>()
                        .Match(some: cmp => cmp.QueryTo().Model<TModel>().Lax().GetValue(),
                               none: () => sel.AsOrDefault<TModel>().Raw
                               );
                }).AddTo(this);
        }

        IObservable<PreviousCurrentPair<Maybe<TModel>, TModel>> ISelectableController<TModel>.ObserveSelected()
        {
            return modelSelection.Where(x => x.IsSome)
                                 .Pairwise()
                                 .Select(pair => PreviousCurrentPair.CreateT(pair.Previous, pair.Current.GetValueUnsafe()));
        }

        IObservable<PreviousCurrentPair<Maybe<TModel>>> ISelectableController<TModel>.ObserveSelection()
        {
            return modelSelection.Pairwise().Select(pair => PreviousCurrentPair.Create(pair.Previous, pair.Current));
        }
    }
}
