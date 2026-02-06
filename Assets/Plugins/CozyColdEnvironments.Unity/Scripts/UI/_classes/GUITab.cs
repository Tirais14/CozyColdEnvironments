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
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable S4144
namespace CCEnvs.Unity.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Showable))]
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

        public Image? image => showable.graphic.As<Image>();

        protected Lazy<InputActionRx<Vector2>> pointerInput { get; private set; } = null!;

        private CommandScheduler commandScheduler = new(UnityFrameProvider.Update);

        protected override void Awake()
        {
            base.Awake();
            SetPointerInput();
        }

        protected override void Start()
        {
            base.Start();
            BindSelectable();
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool SelectableDoSelectPredicate() => true;

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
