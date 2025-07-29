using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UTIRLib.Disposables;
using UTIRLib.GameSystems.Storage;
using UTIRLib.UI.MVVM;
using UTIRLib.Unity.TypeMatching;

namespace UTIRLib.UI.ItemStorage
{
    public class ItemStackViewModel<T> : AViewModel<T>,
        IItemStackViewModel<T>
        where T : IItemStackReactive
    {
        private readonly ReactiveProperty<string> itemCountView = new();
        private readonly ReactiveProperty<Sprite> itemIcon = new();

        public IReadOnlyReactiveProperty<string> ItemCountView => itemCountView;
        public IReadOnlyReactiveProperty<Sprite> ItemIconView => itemIcon;

        public ItemStackViewModel(T model) : base(model)
        {
            model.ItemCountReactive.Subscribe(x => itemCountView.Value = x.ToString())
                                   .AddTo(this);

            model.ItemReactive.Subscribe(x => itemIcon.Value = x.Icon)
                              .AddTo(this);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void OnViewDrop(PointerEventData eventData)
        {
            if (eventData is null)
                throw new ArgumentNullException(nameof(eventData));

            if (eventData.pointerDrag.GetAssignedModel<IItemStack>()
                .IsNot<IItemStack>(out var itemStack)
                )
                return;

            if (itemStack.IsEmpty)
                return;

            if (!model.IsEmpty && !model.Item.Equals(itemStack.Item))
                return;

            if (model.Equals(itemStack))
                return;

            //TODO: Replace this to swap stacks
            if (model.IsFull)
                return;

            model.AddItem(itemStack, itemStack.ItemCount);
        }
    }
}
