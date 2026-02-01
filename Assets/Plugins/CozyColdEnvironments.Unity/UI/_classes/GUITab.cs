#nullable enable
using CCEnvs.Dependencies;
using CCEnvs.FuncLanguage;
using CCEnvs.Patterns.Commands;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using R3;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable S4144
namespace CCEnvs.Unity.UI
{
    [DisallowMultipleComponent]
    public partial class GUITab : CCBehaviour
    {
        [Header("Tab Settings")]
        [Space(8)]

        [SerializeField]
        protected bool switchSelectable = true;

        [field: GetBySelf(IsOptional = true)]
        public Maybe<Button> button { get; private set; }

        [field: GetBySelf(IsOptional = true)]
        public Maybe<Selectable> selectable { get; private set; }

        [field: GetBySelf(IsOptional = true)]
        public Maybe<DragAndDropTarget> dragAndDropTarget { get; private set; }

        [field: GetBySelf(IsOptional = true)]
        public Maybe<Graphic> graphic { get; private set; }

        [field: GetByParent(IsOptional = true)]
        public Maybe<ICanvasController> canvasController { get; private set; }

        [field: GetByParent(IsOptional = true)]
        public Maybe<GUITab> parent { get; private set; }

        [field: GetBySelf(IsOptional = true)]
        public Maybe<CanvasGroup> canvasGroup { get; private set; } = null!;

        [field: GetByParent]
        public Canvas canvas { get; private set; } = null!;

        public Maybe<Image> image => graphic.Raw.As<Image>();
        public Maybe<GUITab> root { get; private set; }

        protected Lazy<InputActionRx<Vector2>> pointerInput { get; private set; } = null!;

        private CommandScheduler commandScheduler = new(UnityFrameProvider.Update);

        protected override void Awake()
        {
            base.Awake();
            SetPointerInput();
            IShowableAwake();
        }

        protected override void Start()
        {
            base.Start();
            InitParentGUITab();
            InitRootGUITab();
            BindSelectable();
            IShowableStart();
        }

        protected virtual void OnTransformChildrenChanged()
        {
        }

        protected virtual void OnTransformParentChanged()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            commandScheduler.Dispose();

            if (button.TryGetValue(out var btn))
                btn.onClick.RemoveAllListeners();

            IShowableOnDestroy();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool SelectableDoSelectPredicate() => true;

        private void InitParentGUITab()
        {
            parent = this.Q()
                .FromParents()
                .ExcludeSelf()
                .Component<GUITab>()
                .Lax();
        }

        private void InitRootGUITab()
        {
            root = this.Q()
                .FromParents()
                .ExcludeSelf()
                .Components<GUITab>()
                .LastOrDefault()
                .Maybe();
        }

        private void BindSelectable()
        {
            if (!this.button.TryGetValue(out Button? button))
                return;

            if (selectable.TryGetValue(out var slct))
            {
                void onClick()
                {
                    if (!slct.IsSelected
                        &&
                        !SelectableDoSelectPredicate()
                        )
                    {
                        return;
                    }

                    if (switchSelectable)
                        slct.SwitchSelectionState();
                    else
                        slct.DoSelect();
                }

                Disposable.Create((button, onClick: (UnityAction)onClick),
                    static input =>
                    {
                        input.button.onClick.RemoveListener(input.onClick);
                    })
                    .RegisterDisposableTo(this);

                button.onClick.AddListener(onClick);
            }
        }

        private void SetPointerInput()
        {
            pointerInput = new Lazy<InputActionRx<Vector2>>(
                static () =>
                {
                    return BuiltInDependecyContainer.Resolve<InputActionRx<Vector2>>(UnityDependecyID.PointerInput);
                });
        }
    }
}
