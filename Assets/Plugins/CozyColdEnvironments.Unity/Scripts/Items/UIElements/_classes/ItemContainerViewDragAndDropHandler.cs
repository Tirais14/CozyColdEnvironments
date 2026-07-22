using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Injections;
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
        private readonly CommandScheduler commandScheduler = CommandScheduler.Update(nameof(ItemContainerViewDragAndDropHandler));

        [SerializeField]
        [Tooltip("Must contains View with " + nameof(IItemContainerViewModel) + ", " + nameof(IShowableElement) + " and " + nameof(IItemContainer) + " as model")]
        private GameObject containerViewProxyPrefab = null!;
        private GameObject? containerViewProxyGO;

        private DragTarget? containerViewProxyDragTarget;

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
            commandScheduler.Enable();
            dragHandler.OnBeginDrag += OnBeginDrag;
            dragHandler.OnDrag += OnDrag;
            dragHandler.OnEndDrag += OnEndDrag;
            dropHandler.OnDrop += OnDrop;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            commandScheduler.Reset();
            commandScheduler.Disable();
            dragHandler.OnBeginDrag -= OnBeginDrag;
            dragHandler.OnDrag -= OnDrag; ;
            dragHandler.OnEndDrag -= OnEndDrag;
            dropHandler.OnDrop -= OnDrop;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            commandScheduler.Dispose();
        }

        public ItemContainerViewDragAndDropHandler SetContainerViewProxyPrefab(GameObject prefab)
        {
            CC.Guard.IsNotNull(prefab, nameof(prefab));
            containerViewProxyPrefab = prefab;
            return this;
        }

        private void OnBeginDragCore()
        {
            if (containerViewProxyGO == null || containerProxy.IsNull())
                return;

            containerViewProxy = containerViewProxyGO.Q().Component<IView>().Strict();
            containerProxy.PutItem(container.TakeItem());
        }

        private void OnBeginDrag(DragContext context)
        {
            containerViewProxyGO = Instantiate(containerViewProxyPrefab);
            containerViewProxyGO.SetActive(true);
            containerViewProxyDragTarget = containerViewProxyGO.AddComponent<DragTarget>();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(OnBeginDrag)
                );

            Command.Builder.WithName(cmdName)
                .WithState(this)
                .WithExecutePredicate(
                static @this =>
                {
                    return @this.containerViewProxyDragTarget != null
                           &&
                           @this.containerViewProxyDragTarget.didStart;
                })
                .WithCancelAction(
                static @this =>
                {
                    if (@this.containerViewProxyGO != null)
                        Destroy(@this.containerViewProxyGO);
                })
                .Synchronously()
                .WithExecuteAction(static @this => @this.OnBeginDragCore())
                .BuildPooled()
                .Value
                .WithCancellationToken(destroyCancellationToken)
                .WithCancellationToken(containerViewProxyDragTarget.destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        private void OnDrag(DragContext context)
        {
            if (containerViewProxyDragTarget == null
                ||
                !containerViewProxyDragTarget.didStart)
            {
                return;
            }

            containerViewProxyDragTarget.SetPosition(context.Event.position);
        }

        private void OnEndDrag(DragContext _)
        {
            if (containerProxy.IsNull())
                return;

            container.PutItem(containerProxy.TakeItem());

            Destroy(containerViewProxyGO);
            containerViewProxy = null;
            containerViewProxyDragTarget = null;
            containerViewProxyGO = null;
        }

        private void OnDrop(DropContext context)
        {
            if (containerProxy.IsNull()
                ||
                !context.TargetGameObject.Q()
                    .Model<IItemContainer>()
                    .Lax()
                    .TryGetValue(out var targetContainer)
                )
            {
                return;
            }

            targetContainer.PutItem(containerProxy.TakeItem());
        }
    }
}
