using CCEnvs.FuncLanguage;
using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.UI.Elements;
using Cysharp.Threading.Tasks;
using UnityEngine;
using IView = CCEnvs.UnityX.UI.IView;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    public class ItemContainerDragHandler : DragHandler
    {
        [SerializeField]
        [Tooltip("Must contains: " + nameof(ItemContainer) + " as model of view")]
        protected GameObject proxyContainerViewPrefab = null!;

        [GetBySelf]
        private IView containerView = null!;
        private IView? proxyContainerView = null;

        protected override void Awake()
        {
            base.Awake();
            Predicate = DragPredicate.Create(this,
                static @this =>
                {
                    return @this.containerView.Maybe()
                        .Map(containerView => containerView.Model)
                        .Cast<IItemContainer>()
                        .Map(container => container.ContainsItem())
                        .GetValue(false);
                });
        }

        protected override void OnBeginEvent(DragEvent ev)
        {
            base.OnBeginEvent(ev);

            if (!IsDragging)
                return;

            DestroyProxyContainerView();

            proxyContainerView = Instantiate(proxyContainerViewPrefab).Q()
                .Component<IView>()
                .Lax()
                .GetValue();

            UniTask.Create()
        }

        private void DestroyProxyContainerView()
        {
            if (proxyContainerView.IsNotNull() &&
                proxyContainerView.HasModel<IItemContainer>() &&
                containerView.HasModel<IItemContainer>())
            {
                var container = containerView.GetModel<IItemContainer>();
                var proxyContainer = proxyContainerView.GetModel<IItemContainer>();

                container.PutItem(proxyContainer.TakeItem());
            }

            proxyContainerView.As<Component>().IfNotNull(cmp => Destroy(cmp));
            proxyContainerView = null;
        }
    }
}
