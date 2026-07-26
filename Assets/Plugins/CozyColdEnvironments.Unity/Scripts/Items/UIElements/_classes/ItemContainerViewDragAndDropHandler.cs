using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.UI;
using CCEnvs.UnityX.UI.Elements;
using Cysharp.Threading.Tasks;
using Humanizer;
using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    /// <summary>
    /// Require <see cref="IDragHandler"/>, <see cref="IDropHandler"/>, container view
    /// </summary>
    public sealed class ItemContainerViewDragAndDropHandler : CCBehaviour
    {
        [SerializeField]
        [Tooltip("Must contains View with " + nameof(IItemContainerViewModel) + ", " + nameof(IShowableElement) + " and " + nameof(IItemContainer) + " as model")]
        private GameObject containerViewProxyPrefab = null!;

        [GetBySelf]
        private IView containerView = null!;
        private IView? containerViewProxy = null!;

        [GetByParent]
        private IDragHandler dragHandler = null!;

        [GetByParent]
        private IDropHandler dropHandler = null!;

        public GameObject ContainerViewProxyGO {
            get => containerViewProxyPrefab;
            set => SetContainerViewProxyPrefab(value);
        }

        private IItemContainer? containerProxy => containerViewProxy.IfNotNull(proxy => proxy.Model.As<IItemContainer>());
        private IItemContainer container => containerView.GetModel<IItemContainer>();

        protected override void OnEnable()
        {
            base.OnEnable();
            dragHandler.OnBeginDrag += OnBeginDrag;
            dragHandler.OnDrag += OnDrag;
            dragHandler.OnEndDrag += OnEndDrag;
            dropHandler.OnDrop += OnDrop;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            dragHandler.OnBeginDrag -= OnBeginDrag;
            dragHandler.OnDrag -= OnDrag; ;
            dragHandler.OnEndDrag -= OnEndDrag;
            dropHandler.OnDrop -= OnDrop;
        }

        public ItemContainerViewDragAndDropHandler SetContainerViewProxyPrefab(GameObject prefab)
        {
            CC.Guard.IsNotNull(prefab, nameof(prefab));
            containerViewProxyPrefab = prefab;
            return this;
        }

        private void OnBeginDrag(DragEvent context)
        {
            containerViewProxy = Instantiate(containerViewProxyPrefab).Q()
                .Component<IView>()
                .Strict();
        }

        private void OnDrag(DragEvent context)
        {
        }

        private void OnEndDrag(DragEvent _)
        {
        }

        private void OnDrop(DropEvent context)
        {
        }
    }
}
