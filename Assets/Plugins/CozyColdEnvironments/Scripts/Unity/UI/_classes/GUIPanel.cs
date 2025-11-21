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
namespace CCEnvs.Unity.UI
{
    [DisallowMultipleComponent]
    public partial class GUIPanel 
        : CCBehaviour,
        IGUIPanel
    {
        [Header("GUI Panel Settings")]
        [Space(8)]

        [SerializeField]
        [GetBySelf(IsOptional = true)]
        protected Image m_Image;

        [SerializeField]
        [GetBySelf(IsOptional = true)]
        protected Button m_Button;

        [SerializeField]
        [GetBySelf(IsOptional = true)]
        protected Selectable m_Selectable;

        [SerializeField]
        [GetBySelf(IsOptional = true)]
        protected DragAndDropTarget m_DragAndDropTarget;

        [SerializeField]
        [Tooltip("If false, only do selection on button click")]
        protected bool switchSelectableOnButtonClick;

        public Maybe<Image> image => m_Image;
        public Maybe<Button> button => m_Button;
        public Maybe<Selectable> selectable => m_Selectable;
        public Maybe<DragAndDropTarget> dragAndDropTarget => m_DragAndDropTarget;

        protected Lazy<ICanvasController> canvasController { get; private set; } = null!;
        protected Lazy<InputActionRx<Vector2>> pointerInput { get; private set; } = null!;

        protected override void Awake()
        {
            base.Awake();

            canvasController = new Lazy<ICanvasController>(
                () => this.QueryTo()
                    .ByParent()
                    .IncludeInactive()
                    .Component<ICanvasController>()
                    .Strict()
                    );

            pointerInput = new Lazy<InputActionRx<Vector2>>(
                () => DependencyContainer.Resolve<InputActionRx<Vector2>>(UnityDependecyID.PointerInput)
                );
        }

        protected override void Start()
        {
            base.Start();
            IShowableStart();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            IShowableOnTransformChildrenChanged();
        }

        protected virtual void OnDestroy()
        {
        }

        private void BindSelectable()
        {
            if (!this.button.TryGetValue(out Button? button))
                return;

            selectable.IfSome(cmp =>
                    button.OnClickAsObservable()
                           .SubscribeWithState2(cmp, this,
                               static (_, cmp, @this) =>
                               {
                                   if (@this.switchSelectableOnButtonClick)
                                       cmp.SwitchSelectionState();
                                   else
                                       cmp.DoSelect();
                               })
                .AddTo(this));
        }
    }
}
