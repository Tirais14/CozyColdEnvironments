using UniRx;
using UnityEngine;

#nullable enable

namespace UTIRLib.UI
{
    [RequireComponent(typeof(ItemStackModelBase))]
    public class ItemStackViewModel : ViewModel<ItemStackModelBase>
    {
        protected readonly ReactiveProperty<Sprite?> itemSpriteView = new();
        protected readonly ReactiveProperty<bool> viewImageEnabled = new();
        protected readonly ReactiveProperty<string> quantityTextView = new();
        protected readonly ReactiveProperty<bool> quantityTextComponentState = new();

        public IReadOnlyReactiveProperty<Sprite?> ItemSpriteView => itemSpriteView;
        public IReadOnlyReactiveProperty<bool> ImageStateView => viewImageEnabled;
        public IReadOnlyReactiveProperty<string> QuantityTextView => quantityTextView;
        public IReadOnlyReactiveProperty<bool> QuantityTextState => quantityTextComponentState;

        public void PutItemStack(IItemStack itemStack)
        {
            if (itemStack == null)
            {
                return;
            }

            model.Put(itemStack);
        }

        /// <returns>Remaining items or <see langword="null"/></returns>
        public IItemStack TakeItemStack(int quantity)
        {
            if (model.IsEmpty) return ItemStack.empty;

            return model.Take(quantity);
        }

        public void SetItemView(IItem? item)
        {
            itemSpriteView.Value = item?.Sprite;
            viewImageEnabled.Value = item != null;
        }

        public virtual void SetQuantityView(int count)
        {
            quantityTextView.Value = count.ToString();
            quantityTextComponentState.Value = count > 0;
        }

        protected override void BindToModel()
        {
            model.ItemProp.Subscribe(SetItemView).AddTo(this);
            model.QuantityProp.Subscribe(SetQuantityView).AddTo(this);
        }
    }
}