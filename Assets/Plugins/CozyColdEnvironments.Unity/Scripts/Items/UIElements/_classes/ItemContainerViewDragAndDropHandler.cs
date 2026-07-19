using CCEnvs.Diagnostics;
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
        [Tooltip("Must contains View with " + nameof(IItemContainerViewModel) + " and " + nameof(IItemContainer) + " as model")]
        private GameObject containerViewProxyPrefab = null!;
        private GameObject? containerViewProxyGO;
        private MonoBehaviour? containerViewProxyGOHook;

        [GetBySelf]
        private IView containerView = null!;
        private IView? containerViewProxy = null!;

        [GetBySelf]
        private IDragHandler dragHandler = null!;

        [GetBySelf]
        private IDropHandler dropHandler = null!;

        private IDisposable? beginDragBinding;
        private IDisposable? dragBinging;
        private IDisposable? endDragBinding;
        private IDisposable? dropBinging;

        public GameObject ContainerViewProxyGO {
            get => containerViewProxyPrefab;
            set => SetContainerViewProxyPrefab(value);
        }

        private IItemContainer containerProxy => containerViewProxy.ThrowIfNull(nameof(containerViewProxy)).GetModel<IItemContainer>();
        private IItemContainer container => containerView.GetModel<IItemContainer>();

        protected override void OnEnable()
        {
            base.OnEnable();
            commandScheduler.Enable();
            BindBeginDrag();
            BindDrag();
            BindEndDrag();
            BindDrop();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            commandScheduler.Reset();
            commandScheduler.Disable();
            beginDragBinding?.Dispose();
            dragBinging?.Dispose();
            endDragBinding?.Dispose();
            dropBinging?.Dispose();
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
            if (containerViewProxyGO == null)
                return;

            containerViewProxy = containerViewProxyGO.Q().Component<IView>().Strict();
            containerProxy.PutItem(container.TakeItem());
        }

        private void OnBeginDrag(DragContext context)
        {
            containerViewProxyGO = Instantiate(containerViewProxyPrefab);
            containerViewProxyGO.SetActive(true);
            containerViewProxyGO.transform.position = new Vector3(-10000, -10000);
            containerViewProxyGOHook = containerViewProxyGO.AddComponent<EmptyMonoBehaviour>();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(OnBeginDrag)
                );

            Command.Builder.WithName(cmdName)
                .WithState(this)
                .WithExecutePredicate(
                static @this =>
                {
                    return @this.containerViewProxyGO == null
                           ||
                           @this.containerViewProxyGOHook!.didStart;
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
                .WithCancellationToken(containerViewProxyGOHook.destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        private void BindBeginDrag()
        {
            beginDragBinding = dragHandler.ObserveBeginDrag().Subscribe(OnBeginDrag);
        }

        private void OnDrag(DragContext context)
        {
            if (containerViewProxyGO == null
                ||
                containerViewProxyGOHook == null
                ||
                !containerViewProxyGOHook.didStart)
            {
                return;
            }

            containerViewProxyGO.transform.position = context.Event.position;
        }

        private void BindDrag()
        {
            dragBinging = dragHandler.ObserveDrag().Subscribe(OnDrag);
        }

        private void OnEndDrag(DragContext _)
        {
            if (containerViewProxyGO == null
                ||
                containerViewProxy.IsNull())
            {
                return;
            }

            container.PutItem(containerProxy.TakeItem());

            containerViewProxy = null;
            Destroy(containerViewProxyGO);
        }

        private void BindEndDrag()
        {
            endDragBinding = dragHandler.ObserveEndDrag().Subscribe(OnEndDrag);
        }

        private void OnDrop(DropContext context)
        {
            if (containerViewProxyGO == null
                ||
                containerViewProxy.IsNull()
                ||
                !context.TargetGameObject.Q().Model<IItemContainer>().Lax().TryGetValue(out var targetContainer))
            {
                return;
            }

            targetContainer.PutItem(containerProxy.TakeItem());
        }

        private void BindDrop()
        {
            dropBinging = dropHandler.ObserveDrop().Subscribe(OnDrop);
        }
    }
}
