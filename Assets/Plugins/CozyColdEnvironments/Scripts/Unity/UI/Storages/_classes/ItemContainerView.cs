using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using CCEnvs.Unity.UI.MVVM;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZLinq;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI.Storages
{
    [RequireComponent(typeof(Image))]
    public abstract class ItemContainerView<TViewModel, TContainer>
        : View<TViewModel, TContainer>,
        IItemContainerView<TViewModel, TContainer>,
        IDragAndDropToggle

        where TViewModel : ViewModel<TContainer>, IItemContainerViewModel<TContainer>
        where TContainer : IItemContainerInfo, new()
    {
        private ICanvasController? canvasController;
        private InputActionRx<Vector2>? pointerInput;
        private List<IDisposable>? disposables;
        private Component? dragging;

        public int DragAndDropHandlerBindingCount { get; private set; }

        [GetBySelf]
        protected Image image { get; private set; } = null!;

        [GetByChildren]
        [field: SerializeField]
        protected TextMeshProUGUI textMesh { get; private set; } = null!;

        protected override void Awake()
        {
            base.Awake();
            
            pointerInput = new LazyCC<InputActionRx<Vector2>>(
                DependencyContainer.Resolve<InputActionRx<Vector2>>(DependencyID.PointerInput));
        }

        protected override void Start()
        {
            base.Start();

            viewModel.ItemIconView.Subscribe(x => image.sprite = x)
                                  .AddTo(this);

            viewModel.ItemCountView.Select(x => x.ToString())
                                   .Subscribe(x => textMesh.text = x)
                                   .AddTo(this);
        }

        protected virtual void OnBegindDrag()
        {
            if (this.As<IView<TViewModel>>()
                           .GetViewModel()
                           .GetModel()
                           .IsEmpty
                           )
                return;

            dragging = Instantiate(this, transform);
        }

        protected virtual void OnDrag()
        {
            cTransform.Value.position = pointerInput!.Value;
        }

        protected virtual void OnEndDrag()
        {
            if (dragging == null)
            {
                this.PrintWarning($"{nameof(dragging)} is null. This is unexpected behaviour for DragAndDrop system, but not critically. Maybe cause memory leaks.");
                return;
            }

            Destroy(dragging.gameObject);
        }

        protected virtual void OnDrop(PointerEventData eventData)
        {
            Guard.IsNotNull(eventData, nameof(eventData));

            foreach (var handler in canvasController!.CanvasRaycaster
                                 .RaycastAll(pointerInput!.Value, this)
                                 .ZL()
                                 .OfType<IView>()
                                 .Where(x => x.GetViewModel().GetModel() is IItemAccessor)
                                 .OfType<IDropHandler>())
            {
                handler.OnDrop(eventData);
            }
        }

        void IDragAndDropToggle.ActivateDragAndDropAbility()
        {
            if (DragAndDropHandlerBindingCount > 0)
            {
                DragAndDropHandlerBindingCount++;
                return;
            }

            if (!this.TryGetAssignedObjectInParent(
                includeInactive: true,
                out canvasController)
                )
                throw new CCException($"Cannot find {nameof(ICanvasController)}.");

            pointerInput = DependencyContainer.Resolve<InputActionRx<Vector2>>(
                DependencyID.PointerInput);

            if (!TryGetComponent<DragHandler>(out var dragHandler))
            {
                this.PrintError($"Cannot find {nameof(DragHandler)}. DragAndDrop is not working.");
                return;
            }

            if (!TryGetComponent<DropHandler>(out var dropHandler))
            {
                this.PrintError($"Cannot find {nameof(DropHandler)}. DragAndDrop is not working.");
                return;
            }

            disposables = new List<IDisposable>
            {
                dragHandler.OnBeginDragRx.Subscribe(_ => OnBegindDrag()),
                dragHandler.OnDragRx.Subscribe(_ => OnDrag()),
                dragHandler.OnEndDragRx.Subscribe(_ => OnEndDrag()),

                dropHandler.OnDropRx.Subscribe(OnDrop)
            };

            DragAndDropHandlerBindingCount++;
        }

        void IDragAndDropToggle.DeactivateDragAndDropAbility()
        {
            if (DragAndDropHandlerBindingCount <= 0)
                return;

            canvasController = null;
            pointerInput = null;

            disposables!.ForEach(x => x.Dispose());
            disposables = null;

            DragAndDropHandlerBindingCount--;
        }
    }
}
