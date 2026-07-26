using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.UI;
using CCEnvs.UnityX.UI.Elements;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    [DisallowMultipleComponent]
    public sealed class ItemContainerViewDropHandler : CCBehaviour
    {
        [GetBySelf]
        private IDropHandler dropHandler = null!;

        [GetBySelf]
        private IView containerView = null!;

        protected override void OnEnable()
        {
            base.OnEnable();
            dropHandler.OnDrop += OnDrop;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            dropHandler.OnDrop -= OnDrop;
        }

        private void OnDrop(DropEvent context)
        {
            if (!containerView.HasModel<IItemContainer>()||
                !context.TargetGameObject.Q()
                    .Model<IItemContainer>()
                    .Lax()
                    .TryGetValue(out var dragContainer)
                )
            {
                return;
            }

            dragContainer.PutItem(containerView.GetModel<IItemContainer>().PutItem(dragContainer.TakeItem()));
        }
    }
}
