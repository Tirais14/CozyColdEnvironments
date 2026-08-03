using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Items;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    [DisallowMultipleComponent]
    public sealed class ItemContainerViewDragHandler : ShowableDragHandler
    {
        [GetBySelf]
        private IView containerView = null!;

        protected override void Awake()
        {
            base.Awake();
            Predicate = DragPredicate.Create(
                this,
                static @this =>
                {
                    return @this.containerView.HasModel<IItemContainer>() &&
                           @this.containerView.GetModel<IItemContainer>().ContainsItem();
                });
        }

        protected override void OnBeginDragEvent(DragEvent ev)
        {
            base.OnBeginDragEvent(ev);

            if (!containerView.HasModel<IItemContainer>() ||
                ev.TargetGameObject == null ||
                !ev.TargetGameObject.Q()
                    .Model<IItemContainer>()
                    .Lax()
                    .TryGetValue(out var dragContainer)
                    )
            {
                return;
            }

            var container = containerView.GetModel<IItemContainer>();
            container.TakeItem().PutItemTo(dragContainer).PutItemTo(container);
        }

        protected override void OnEndDragEvent(DragEvent ev)
        {
            base.OnEndDragEvent(ev);

            if (!IsDragging ||
                !containerView.HasModel<IItemContainer>() ||
                ev.TargetGameObject == null ||
                !ev.TargetGameObject.Q()
                    .Model<IItemContainer>()
                    .Lax()
                    .TryGetValue(out var dragContainer))
            {
                return;
            }

            var container = containerView.GetModel<IItemContainer>();
            dragContainer.TakeItem().PutItemTo(container).PutItemTo(dragContainer);
        }
    }
}
