#nullable enable
using CCEnvs.Dependencies;
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Commands;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Dependencies;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using Cysharp.Threading.Tasks;
using System;
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
        protected readonly CommandScheduler commandScheduler = new();

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
        [Tooltip("If false, only do selection on button click")]
        protected bool switchSelectableOnButtonClick = true;

        public Maybe<Image> image => m_Graphic.As<Image>();
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
                    .FromParents()
                    .IncludeInactive()
                    .Component<ICanvasController>()
                    .Strict()
                    );

            pointerInput = new Lazy<InputActionRx<Vector2>>(
                () => DependencyContainer.Resolve<InputActionRx<Vector2>>(UnityDependecyID.PointerInput)
                );

            commandScheduler.Start(
                PlayerLoopTiming.LastInitialization,
                cancellationToken: destroyCancellationToken
                )
                .AddTo(this);

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
            IShowableOnTransformChildrenChanged();
        }

        protected virtual void OnTransformParentChanged()
        {

        }

        protected virtual void OnDestroy()
        {
        }

        //private static async UniTask RunCommandScheduler(GUITab instance)
        //{
        //    while (!instance.destroyCancellationToken.IsCancellationRequested)
        //    {
        //        await UniTask.WaitForEndOfFrame(cancellationToken: instance.destroyCancellationToken);
        //        if (instance.enabled)
        //            instance.commandScheduler.DoTick();

        //        await UniTask.NextFrame(
        //            PlayerLoopTiming.LastInitialization, 
        //            cancellationToken: instance.destroyCancellationToken
        //            );
        //    }
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<IGUITab> GetParentGUI()
        {
            return this.QueryTo()
                       .FromParents()
                       .ExcludeSelf()
                       .Component<IGUITab>()
                       .Lax();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsGuiChildOf(IGUITab guiTab)
        {
            CC.Guard.IsNotNull(guiTab, nameof(guiTab));

            return GetParentGUI().Map(parent => parent.Is<MonoBehaviour>(out var mono) && transform.IsChildOf(mono.transform))
                                 .GetValue(false);
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
                      
                              if (@this.switchSelectableOnButtonClick)
                                  cmp.SwitchSelectionState();
                              else
                                  cmp.DoSelect();
                          })
                      .AddTo(this);
            }
        }
    }
}
