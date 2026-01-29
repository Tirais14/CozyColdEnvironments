using CCEnvs.FuncLanguage;
using CCEnvs.Unity.UI;
using System;
using System.Linq;
using R3;
using UnityEngine;
using ZLinq;
using CCEnvs.Unity.Components;

#nullable enable
namespace CCEnvs.Unity
{
    public class ModelSelectableController<TModel> : SelectableController<ISelectable>, ISelectableController<TModel>
    {
        protected readonly ReactiveProperty<Maybe<TModel>> modelSelection = new();

        Maybe<TModel> ISelectableController<TModel>.Selection => modelSelection.Value;

        protected override void Awake()
        {
            base.Awake();

            selection.Select(slct => slct.Raw.As<Component>())
                .Subscribe(this,
                static (mCmp, @this) =>
                {
                    if (mCmp.TryGetValue(out Component? cmp))
                        @this.modelSelection.Value = cmp.Q().Model<TModel>().Lax();
                    else
                        @this.modelSelection.Value = cmp.As<TModel>();
                })
                .RegisterDisposableTo(this);
        }

        Observable<TModel> ISelectableController<TModel>.ObserveDeselected()
        {
            return modelSelection.Pairwise()
                                 .Where(static pair => pair.Current.IsNone && pair.Previous.IsSome)
                                 .Select(static pair => pair.Previous.GetValueUnsafe());
        }

        Observable<TModel> ISelectableController<TModel>.ObserveSelected()
        {
            return modelSelection.Where(static x => x.IsSome).Select(x => x.GetValueUnsafe());
        }

        Observable<Maybe<TModel>> ISelectableController<TModel>.ObserveSelection()
        {
            return modelSelection;
        }
    }
}
