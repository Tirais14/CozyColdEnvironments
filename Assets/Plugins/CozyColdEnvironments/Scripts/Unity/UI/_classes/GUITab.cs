#nullable enable
using CCEnvs.Dependencies;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;
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

        protected virtual void OnDestroy()
        {
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
                button.OnClickAsObservable()
                      .SubscribeWithState2(slct, this,
                          static (_, cmp, @this) =>
                          {
                              if (!cmp.IsSelected
                                  &&
                                  !@this.SelectableDoSelectPredicate()
                                  )
                                  return;

                              if (@this.switchSelectable)
                                  cmp.SwitchSelectionState();
                              else
                                  cmp.DoSelect();

                          })
                      .AddTo(this);
            }
        }
    }
}
