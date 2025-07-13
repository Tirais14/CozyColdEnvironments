using UniRx;
using UnityEngine;

#nullable enable

namespace UTIRLib.UI
{
    [RequireComponent(typeof(DragItemStack))]
    public class DragItemStackViewModel : ViewModel<DragItemStack>
    {
        protected readonly ReactiveProperty<Sprite> spriteView = new();

        public IReadOnlyReactiveProperty<bool> IsActiveView => model.IsActive;
        public Sprite? SpriteView => model.ItemStack.Item?.Sprite;

        protected override void BindToModel()
        { }
    }
}