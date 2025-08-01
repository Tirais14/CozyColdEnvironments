using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UTIRLib.Disposables;
using UTIRLib.GameSystems.ItemStorageSystem;
using UTIRLib.UI.MVVM;
using UTIRLib.Unity.Extensions;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib.UI.ItemStorageSystem
{
    public class ItemStackViewModel<T> : AViewModel<T>,
        IItemStackViewModel<T>
        where T : IItemStackReactive
    {
        private readonly ReactiveProperty<string> counterView = new();
        private readonly ReactiveProperty<Sprite?> iconView = new();

        public IReadOnlyReactiveProperty<string> CounterView => counterView;
        public IReadOnlyReactiveProperty<Sprite?> IconView => iconView;

        public ItemStackViewModel(T model) : base(model)
        {
            model.ItemCountReactive.Subscribe(x => counterView.Value = x.ToString())
                                   .AddTo(this);

            model.ItemReactive.Subscribe(x => iconView.Value = x.IfNotNull(x => x.Icon)
                                                                .IfNull(TirLib.ErrorSprite))
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

            if (!model.IsEmpty && !model.Item!.Equals(itemStack.Item))
                return;

            if (model.Equals(itemStack))
                return;

            //TODO: Replace this to swap stacks
            if (model.IsFull)
                return;

            model.AddItemFrom(itemStack, itemStack.ItemCount);
        }
    }
}
