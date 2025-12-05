using CCEnvs.Patterns.Commands;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Initables;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public partial class GUITab : IShowable
    {
        private readonly HashSet<GraphicStateSnaphsot> graphicStates = new(
            new AnonymousEqualityComparer<GraphicStateSnaphsot>(
            (left, right) => left.Target.Equals(right.Target),
            x => x.Target.GetHashCode()
            ));

        private readonly HashSet<ShowableStateSnapshot> showableStates = new(
            new AnonymousEqualityComparer<ShowableStateSnapshot>(
            (left, right) => left.Target.Equals(right.Target),
            x => x.Target.GetHashCode()
            ));

        private readonly HashSet<Transform> controlledChilds = new();

        private readonly ReactiveProperty<bool> isShown = new(true);
        private bool redrawScheduled;

        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_ShowOnInited;

        public bool ShowOnInited {
            get => m_ShowOnInited;
            set => m_ShowOnInited = value;
        }

        public bool IsShown => isShown.Value;

        public bool IsVisible {
            get
            {
                return isShown.Value
                       &&
                       isActiveAndEnabled
                       &&
                       GetParentGui().Map(gui => gui.IsVisible).GetValue(true);
            }
        }

        public virtual bool ShowAllowed => IsInited;
        public virtual bool HideAllowed => IsInited;
        public bool IsInited { get; private set; }

        private void IShowableAwake()
        {
            isShown.AddTo(this);
        }

        private void IShowableStart()
        {
            if (GetParentGui().IsNone)
                transform.position *= 100;

            this.DoActionAsync(static async (@this) =>
            {
                var childs = @this.Q()
                                  .ExcludeSelf()
                                  .ByChildren()
                                  .Components<IShowable>()
                                  .ToArray();

                if (childs.IsNotEmpty())
                    await UniTask.WaitUntil(childs, static childs => childs.All(x => x.IsInited));

                if (@this.GetParentGui().IsNone)
                {
                    @this.transform.position /= 100;
                    @this.HideInternal();
                }

                @this.IsInited = true;
            });

            //this.DoActionAsync(Init);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IShowableOnTransformChildrenChanged()
        {
            //if (IsInited)
            //    Redraw();

            //if (redrawScheduled)
            //    return;

            //redrawScheduled = true;
            //this.DoActionAsync(static async @this =>
            //{
            //    if (!@this.IsInited)
            //        await UniTask.WaitUntil(@this,
            //            static @this => @this.IsInited,
            //            timing: PlayerLoopTiming.PreUpdate
            //            );

            //    await UniTask.WaitForEndOfFrame();

            //    try
            //    {
            //        @this.Redraw();
            //    }
            //    catch (Exception ex)
            //    {
            //        @this.PrintException(ex);
            //    }
            //    finally
            //    {
            //        @this.redrawScheduled = false;
            //    }
            //});
        }

        private static async UniTask Init(GUITab @this)
        {


            //if (@this.ShowOnInited)
            //    @this.ShowInternal();

            //var childs = @this.Q()
            //                  .ByChildren()
            //                  .ExcludeSelf()
            //                  .Components<IInitableBase>()
            //                  .ZLinq()
            //                  .Where(x => x.Is<IShowable>())
            //                  .ToArray();

            //if (childs.IsNotEmpty())
            //{
            //    await UniTask.WaitUntil(
            //        childs,
            //        static childs => childs.All(x => x.IsInited),
            //        timing: PlayerLoopTiming.LastInitialization
            //        );
            //}

            //@this.IsInited = true;
            //if (@this.m_Graphic != null)
            //{
            //    var parents = @this.Q()
            //                       .ByParent()
            //                       .ExcludeSelf()
            //                       .Components<IInitableBase>()
            //                       .ZLinq()
            //                       .Where(x => x.Is<IShowable>())
            //                       .ToArray();

            //    if (parents.IsNotEmpty())
            //    {
            //        await UniTask.WaitUntil(
            //            parents,
            //            static parents => parents.All(x => x.IsInited),
            //            timing: PlayerLoopTiming.LastInitialization
            //            );
            //    }

            //    UIHelper.UndoTransparentRecursive(@this.m_Graphic);
            //}

            //if (!@this.ShowOnInited
            //    &&
            //    @this.GetParentGui().IsNone
            //    )
            //    @this.HideInternal(@this);
        }

        public void Hide()
        {
            if (!IsShown)
                return;

            var command = Command.Create(this,
                isReadyToExecute: static @this => @this.HideAllowed,
                execute: static @this => @this.HideInternal(),
                name: nameof(Hide),
                undoCommandsOnAdd: Range.From(new CommandInfo(commandName: nameof(Show))),
                singleCommand: true
                );

            commandScheduler.AddCommand(command);
        }

        public void Show()
        {
            if (IsShown)
                return;

            var command = Command.Create(this,
                isReadyToExecute: static @this => @this.ShowAllowed,
                execute: static @this => @this.ShowInternal(),
                name: nameof(Show),
                singleCommand: true
                );

            commandScheduler.AddCommand(command);
        }

        public bool SwitchShownState()
        {
            if (IsShown)
                Hide();
            else
                Show();

            return IsShown;
        }

        public void SwitchShownStateVoid() => SwitchShownState();

        public void Redraw()
        {
            //if (!IsShown)
            //    Hide();
        }

        public void OnAddChildren(Transform child)
        {
            CC.Guard.IsNotNull(child, nameof(child));

            var cmps = transform.Q().Components<Component>();

            //if (cmps.Any(x => x.Is<IShowable>()))
            //    return;

            var graphics = cmps.ZLinq().OfType<Graphic>();
            graphics.ForEach(x => x.DoTransparent());

            this.DoActionAsync(graphics,
                static async graphics =>
                {
                    await UniTask.NextFrame(timing: PlayerLoopTiming.Initialization);
                    graphics.ForEach(x => x.UndoTransparent());
                });
        }

        public IObservable<Unit> ObserveShow()
        {
            return isShown.Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveHide()
        {
            return isShown.Where(x => !x).AsUnitObservable();
        }

        public void InitShowable()
        {
            if (IsInited)
                return;

            if (GetParentGui().TryGetValue(out var parentGUI))
                parentGUI.InitShowable();
            else
                HideInternal();

            IsInited = true;
        }

        protected void HideInternal()
        {
            OnHide();

            foreach (var graphic in graphicStates.ZLinq().Select(x => x.Target))
                DisableGraphics(graphic);

            foreach (var showableState in showableStates.ZLinq().Where(x => x.IsShown).Select(x => x.Target))
                showableState.Hide();

            OnHiden();
        }

        protected virtual void OnHide()
        {
            UIHelper.CaptureGraphicStatesUntilShowable(gameObject, graphicStates);
            UIHelper.CaptureShowableStatesUntilShowable(gameObject, showableStates);
        }

        protected virtual void OnHiden()
        {
            isShown.Value = false;
        }

        protected virtual void ShowInternal()
        {
            OnShow();
            graphicStates.RestoreStates();
            graphicStates.Clear();
            showableStates.RestoreStates();
            showableStates.Clear();
            OnShown();
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnShown()
        {
            isShown.Value = true;
        }

        protected virtual void DisableGraphics(Graphic graphic)
        {
            if (graphic == null)
                return;

            graphic.enabled = false;
        }
    }
}
