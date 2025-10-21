using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using CCEnvs.Unity.UI.MVVM;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZLinq;


#nullable enable
#pragma warning disable IDE0044
#pragma warning disable IDE1006
#pragma warning disable IDE0051
#pragma warning disable S3236
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
        private DragAndDropToggle dragAndDropToggle;
        private InputActionRx<Vector2>? pointerInput;
        private Component? dragItem;

        [GetByParent]
        protected ICanvasController canvasController { get; private set; } = null!;

        [GetBySelf]
        protected Image image { get; private set; } = null!;

        [GetByChildren]
        [field: SerializeField]
        protected TextMeshProUGUI textMesh { get; private set; } = null!;

        int IDragAndDropToggle.BindingCount => dragAndDropToggle.BindingCount;

        protected override void Awake()
        {
            base.Awake();
            
            pointerInput = new LazyCC<InputActionRx<Vector2>>(
                DependencyContainer.Resolve<InputActionRx<Vector2>>(DependencyID.PointerInput));

            dragAndDropToggle = new DragAndDropToggle(
                gameObject,
                onBeginDrag: _ => OnBegindDrag(),
                onDrag: _ => OnDrag(),
                onEndDrag: _ => OnEndDrag(),
                onDrop: OnDrop);
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

            dragItem = Instantiate(this, transform);
        }

        protected virtual void OnDrag()
        {
            cTransform.Value.position = pointerInput!.Value;
        }

        protected virtual void OnEndDrag()
        {
            if (dragItem == null)
            {
                this.PrintWarning($"{nameof(dragItem)} is null. This is unexpected behaviour for DragAndDrop system, but not critically. Maybe cause memory leaks.");
                return;
            }

            Destroy(dragItem.gameObject);
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
            dragAndDropToggle.ActivateDragAndDropAbility();
        }

        void IDragAndDropToggle.DeactivateDragAndDropAbility()
        {
            dragAndDropToggle.DeactivateDragAndDropAbility();
        }
    }
}
