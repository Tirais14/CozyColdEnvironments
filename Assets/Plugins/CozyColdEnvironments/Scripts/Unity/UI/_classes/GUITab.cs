#nullable enable
using CCEnvs.Dependencies;
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable S4144
namespace CCEnvs.Unity.UI
{
    [DisallowMultipleComponent]
    public partial class GUITab 
        : CCBehaviour,
        IGUITab
    {
        [Header("Tab Settings")]
        [Space(8)]

        [SerializeField]
        [GetBySelf(IsOptional = true)]
        protected Graphic? m_Graphic;

        [SerializeField]
        [GetBySelf(IsOptional = true)]
        protected Button? m_Button;

        [SerializeField]
        [GetBySelf(IsOptional = true)]
        protected Selectable? m_Selectable;

        [SerializeField]
        [GetBySelf(IsOptional = true)]
        protected DragAndDropTarget? m_DragAndDropTarget;

        [SerializeField]
        protected bool switchSelectable = true;

        public Maybe<Image> image => m_Graphic.As<Image>();
        public Maybe<Button> button => m_Button;
        public Maybe<Selectable> selectable => m_Selectable;
        public Maybe<DragAndDropTarget> dragAndDropTarget => m_DragAndDropTarget;
        public Maybe<Material> material => m_Graphic.Maybe().Map(x => x.material);
        public Maybe<Graphic> graphic => m_Graphic;

        protected Lazy<ICanvasController> canvasController { get; private set; } = null!;
        protected Lazy<InputActionRx<Vector2>> pointerInput { get; private set; } = null!;

        protected override void Awake()
        {
            base.Awake();

            canvasController = new Lazy<ICanvasController>(
                () => this.QueryTo()
                    .FromParents()
                    .IncludeInactive()
                    .Component<ICanvasController>()
                    .Strict()
                    );

            pointerInput = new Lazy<InputActionRx<Vector2>>(
                static () => DependencyContainer.Resolve<InputActionRx<Vector2>>(UnityDependecyID.PointerInput)
                );

            IShowableAwake();
        }

        protected override void Start()
        {
            base.Start();
            IShowableStart();
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

            if (button.TryGetValue(out var btn))
                btn.onClick.RemoveAllListeners();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<IGUITab> GetParentGUI()
        {
            return this.Q()
                       .FromParents()
                       .ExcludeSelf()
                       .Component<IGUITab>()
                       .Lax();
        }

        public Maybe<IGUITab> GetRootGUI()
        {
            return this.Q()
                       .FromParents()
                       .ExcludeSelf()
                       .Components<IGUITab>()
                       .LastOrDefault()
                       .Maybe();
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
                    .BindDisposableTo(this);

                button.onClick.AddListener(onClick);
            }
        }
    }
}
