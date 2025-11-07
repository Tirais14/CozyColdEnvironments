#nullable enable
using CCEnvs.Dependencies;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using System;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable S4144
namespace CCEnvs.Unity.UI.Elements
{
    [DisallowMultipleComponent]
    public partial class ViewElement 
        : CCBehaviour,
        IViewElement
    {
        [field: SerializeField, GetBySelf]
        public Maybe<Image> Img { get; private set; }

        public Maybe<IViewElement> ParentViewElement { get; private set; }

        protected Lazy<ICanvasController> canvasController { get; private set; } = null!;
        protected Lazy<InputActionRx<Vector2>> pointerInput { get; private set; } = null!;

        protected bool parentIsVisible => ParentViewElement.Match(
            some: parent => parent.IsVisible,
            none: () => true).Raw;

        protected override void Awake()
        {
            base.Awake();

            canvasController = new Lazy<ICanvasController>(
                () => this.FindFor()
                          .InParent()
                          .IncludeInactive()
                          .Component<ICanvasController>()
                          .Strict()
                );

            pointerInput = new Lazy<InputActionRx<Vector2>>(
                () => DependencyContainer.Resolve<InputActionRx<Vector2>>(UnityDependecyID.PointerInput)
                );

            AwakeDragAndDrop();
        }

        protected override void Start()
        {
            base.Start();

            StartIShowable();
            StartISelectable();
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnTransformParentChanged()
        {
            ResolveParent();
        }

        protected void ResolveParent()
        {
            ParentViewElement = this.FindFor().InParent().Component<IViewElement>();
        }
    }
}
