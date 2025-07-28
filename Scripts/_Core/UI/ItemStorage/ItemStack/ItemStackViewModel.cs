using UniRx;
using UnityEngine;
using UTIRLib.Disposables;
using UTIRLib.UI.MVVM;

namespace UTIRLib.UI.ItemStorage
{
    public class ItemStackViewModel : AViewModel<ItemStackReactive>
    {
        private readonly ReactiveProperty<string> itemCountView = new();
        private readonly ReactiveProperty<Sprite> itemIcon = new();

        public IReadOnlyReactiveProperty<string> ItemCountView => itemCountView;
        public IReadOnlyReactiveProperty<Sprite> ItemIconView => itemIcon;

        public ItemStackViewModel(ItemStackReactive model) : base(model)
        {
            model.ItemCountReactive.Subscribe(x => itemCountView.Value = x.ToString())
                                   .AddTo(this);

            model.ItemReactive.Subscribe(x => itemIcon.Value = x.Icon)
                              .AddTo(this);
        }
    }
}
