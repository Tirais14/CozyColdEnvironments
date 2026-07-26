using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Items;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    [DisallowMultipleComponent]
    public sealed class ItemContainerViewDragHandler : CCBehaviour
    {
        [GetByParent]
        private IDragHandler dragHandler = null!;

        [GetBySelf]
        private IView containerView = null!;

        public bool IsDragging { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            dragHandler.Predicate = DragPredicate.Create(
                this,
                static @this =>
                {
                    return @this.containerView.HasModel<IItemContainer>() &&
                           @this.containerView.GetModel<IItemContainer>().ContainsItem();
                });
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            dragHandler.OnBeginDrag += OnBeginDrag;
            dragHandler.OnEndDrag += OnEndDrag;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            dragHandler.OnBeginDrag -= OnBeginDrag;
            dragHandler.OnEndDrag -= OnEndDrag;
        }

        private void OnBeginDrag(DragEvent ev)
        {
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

            IItemContainer container = containerView.GetModel<IItemContainer>();

            container.PutItem(dragContainer.PutItem(container.TakeItem()));
            IsDragging = true;
        }

        private void OnEndDrag(DragEvent ev)
        {
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
            dragContainer.PutItem(container.PutItem(dragContainer.TakeItem()));
            IsDragging = false;
        }
    }
}
