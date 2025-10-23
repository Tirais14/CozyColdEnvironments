using CCEnvs.Dependencies;
using CCEnvs.Language;
using CCEnvs.Reflection.Data;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.InputSystem.Rx;
using CCEnvs.Unity.UI.Elements;
using CommunityToolkit.Diagnostics;
using System;
using System.Linq;
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

#pragma warning disable S1117
namespace CCEnvs.Unity.UI.MVVM
{
    public static class View
    {
        public static T[] FindViewsByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
            where T : IView
        {
            return SceneSearch.FindObjectsByType<T>(findObjectsInactive,
                                                    findObjectsSortMode);
        }

        public static T? FindAnyViewByType<T>(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
            where T : IView
        {
            return SceneSearch.FindObjectsByType<T>(
                findObjectsInactive,
                findObjectsSortMode)
                .FirstOrDefault();
        }
    }
    /// <summary>
    /// <see cref="TModel"/> must have empty constructor by default
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public abstract class View<TViewModel, TModel>
        : Element,
        IView<TViewModel, TModel>,
        IDragAndDropToggle

        where TViewModel : ViewModel<TModel>
    {
        private Lazy<TViewModel> _viewModel;

        protected Ghost<Image> image { get; private set; }
        protected TModel model;
        protected TViewModel viewModel => _viewModel.Value;
        protected Lazy<ICanvasController> canvasController { get; private set; } = null!;
        protected Lazy<DragAndDropToggle> dragAndDropToggle { get; private set; } = null!;
        protected Lazy<InputActionRx<Vector2>> pointerInput { get; private set; } = null!;
        protected virtual Ghost<Component> dragItem => this;
        protected virtual bool readyToDrag => false;
        protected virtual bool readyToTakeDrop => false;

        int IDragAndDropToggle.BindingCount => dragAndDropToggle.Value.BindingCount;

        protected override void Awake()
        {
            base.Awake();

            image = GetComponent<Image>();

            _viewModel = new LazyCC<TViewModel>(CreateViewModel);

            canvasController = new Lazy<ICanvasController>(
                () => this.GetAssignedObjectInParent<ICanvasController>(includeInactive: true)
                .ValidateGetOperation()
                );

            dragAndDropToggle = new Lazy<DragAndDropToggle>(
                () => new DragAndDropToggle(
                    gameObject,
                    OnBeginDrag,
                    OnDrag,
                    OnEndDrag,
                    OnDrop)
                );

            pointerInput = new Lazy<InputActionRx<Vector2>>(
                () => DependencyContainer.Resolve<InputActionRx<Vector2>>(DependencyID.PointerInput)
                );
        }

        public TViewModel GetViewModel()
        {
            _viewModel ??= new LazyCC<TViewModel>(CreateViewModel);

            return _viewModel.Value;
        }

        public TModel GetModel() => GetViewModel().GetModel();

        protected virtual TModel CreateModel()
        {
            var model = InstanceFactory.Create<TModel>(
                parameters: InstanceFactory.Parameters.CacheConstructor
                            |
                            InstanceFactory.Parameters.ThrowIfNotFound)!;

            if (model is IDisposable disposable)
                disposable.AddTo(this);

            return model;
        }

        protected virtual TViewModel CreateViewModel()
        {
            TModel model = CreateModel();

#if UNITY_2017_1_OR_NEWER
            return InstanceFactory.Create<TViewModel>(
                new ExplicitArguments(
                    new ExplicitArgument(model!),
                    new ExplicitArgument(gameObject)),
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);
#else
            return InstanceFactory.Create<TViewModel>(
                new ExplicitArguments(
                    new ExplicitArgument(model!)),
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);
#endif //UNITY_2017_1_OR_NEWER
        }

        protected virtual void OnBeginDrag(PointerEventData eventData)
        {
        }

        protected virtual void OnDrag(PointerEventData eventData)
        {
            if (!readyToDrag)
                return;

            cTransform.Value.position = pointerInput.Value.InputValue;
        }

        protected virtual void OnEndDrag(PointerEventData eventData)
        {
        }

        protected virtual void OnDrop(PointerEventData eventData)
        {
        }

        void IDragAndDropToggle.ActivateDragAndDropAbility()
        {
            dragAndDropToggle.Value.ActivateDragAndDropAbility();
        }

        void IDragAndDropToggle.DeactivateDragAndDropAbility()
        {
            dragAndDropToggle.Value.DeactivateDragAndDropAbility();
        }
    }
}
