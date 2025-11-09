#nullable enable
using CCEnvs.Dependencies;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using System;
using UniRx;
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
        public Maybe<Image> image { get; private set; }

        [field: SerializeField, GetBySelf]
        public Maybe<Button> button { get; private set; }

        protected Lazy<ICanvasController> canvasController { get; private set; } = null!;
        protected Lazy<InputActionRx<Vector2>> pointerInput { get; private set; } = null!;

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

            AwakeIDragAndDropTarget();
        }

        protected override void Start()
        {
            base.Start();
            BindToButton();
            StartIShowable();
            StartISelectable();
        }

        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        /// By default will be invoked on <see cref="Button.onClick"/> event
        /// <br/>Work only if parent <see cref="GameObject"/> has <see cref="Button"/> before <see cref="Awake"/>
        /// </summary>
        public virtual void OnButtonClick()
        {
        }

        private void BindToButton()
        {
            button.IfSome(button =>
            {
                button.OnClickAsObservable()
                    .SubscribeWithState(this, (_, view) => view.OnButtonClick())
                    .AddTo(this);
            });
        }
    }
}
