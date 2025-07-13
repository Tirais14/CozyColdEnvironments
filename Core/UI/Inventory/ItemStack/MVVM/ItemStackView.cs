using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UTIRLib.Attributes;
using UTIRLib.Diagnostics;
using UTIRLib.Injector;

#nullable enable

namespace UTIRLib.UI
{
    [RequireComponent(typeof(Image), typeof(ItemStackViewModel))]
    public class ItemStackView : View<ItemStackViewModel>, IDragInteractable<IItemStack>
    {
        [Optional]
        [SerializeField]
        [GetComponentInChildrenIfNull]
        protected TextMeshProUGUI? text = null;

        public void Drag(IItemStack itemStack) => viewModel.PutItemStack(itemStack);

        protected void SetSprite(Sprite? sprite) => image.sprite = sprite;

        protected void SetQuantityText(string quantityString) =>
            text.IfNotNull((self) => self.text = quantityString);

        protected void SetQuantityTextComponentState(bool isEnabled) =>
            text.IfNotNull((self) => self.enabled = isEnabled);

        protected override void BindToViewModel()
        {
            viewModel.ItemSpriteView.Subscribe(SetSprite).AddTo(this);
            viewModel.ImageStateView.Subscribe(SetImageEnabledState).AddTo(this);
            if (text != null)
            {
                viewModel.QuantityTextView.Subscribe(SetQuantityText).AddTo(this);
                viewModel.QuantityTextState.Subscribe(SetQuantityTextComponentState).AddTo(this);
            }
        }
    }
}