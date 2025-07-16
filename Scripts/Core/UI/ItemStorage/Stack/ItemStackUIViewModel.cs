using UniRx;
using UnityEngine;

#nullable enable
namespace UTIRLib.UI
{
    public class ItemStackUIViewModel : ViewModel, 
        IItemStackUIViewModel,
        IViewModel<IItemStackUIReactive>
    {
        private readonly ReactiveProperty<Sprite?> itemIcon = new();
        private readonly ReactiveProperty<string> itemCount = new();
        private IItemStackUIReactive model = null!;

        public IReadOnlyReactiveProperty<Sprite?> ItemIcon => itemIcon;
        public IReadOnlyReactiveProperty<string> ItemCount => itemCount;
        IItemStackUIReactive IViewModel<IItemStackUIReactive>.Model => model;

        public ItemStackUIViewModel(IItemStackUIReactive model)
        {
            this.model = model;

            model.Item.Subscribe(x => itemIcon.Value = x.Icon).AddTo(this);
            model.ItemCount.Subscribe(x => itemCount.Value = x.ToString()).AddTo(this);
        }
    }
}
