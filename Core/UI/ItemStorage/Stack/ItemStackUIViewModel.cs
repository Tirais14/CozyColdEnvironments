using UniRx;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Injector;
using System;

#nullable enable
namespace UTIRLib.UI
{
    [RequireComponent(typeof(ItemStackUI))]
    public class ItemStackUIViewModel : MonoX
    {
        private readonly ReactiveProperty<Sprite?> iconView = new();
        private readonly ReactiveProperty<string> countView = new();

        [GetComponent]
        private ItemStackUIModel model = null!;

        public IReadOnlyReactiveProperty<Sprite?> IconView => iconView;
        public IReadOnlyReactiveProperty<string> CountView => countView;

        protected override void OnStart()
        {
            base.OnStart();

            model.ItemProp.Subscribe(OnItemChanged).AddTo(this);
            model.ItemCountProp.Subscribe(OnItemCountChanged).AddTo(this);
        }

        /// <exception cref="ArgumentNullException"></exception>
        private void OnItemChanged(IItemUI item)
        {
            if (item.IsNull())
                throw new ArgumentNullException(nameof(item));

            if (item is ItemUIEmpty)
            {
                iconView.Value = null;

                return;
            }

            iconView.Value = item.Icon;
        }

        private void OnItemCountChanged(int count)
        {
            countView.Value = count.ToString();
        }
    }
}
