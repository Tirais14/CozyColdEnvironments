using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.UI;
using CCEnvs.UnityX.UI.Elements;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    [DisallowMultipleComponent]
    public sealed class ItemContainerViewDropHandler : UnityX.UI.Elements.DropHandler
    {
        [GetBySelf]
        private IView containerView = null!;

        protected override void OnDropEvent(DropEvent ev)
        {
            base.OnDropEvent(ev);

            if (!containerView.HasModel<IItemContainer>() ||
                !ev.TargetGameObject.Q()
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
