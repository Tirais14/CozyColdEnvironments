using CCEnvs.Patterns.Commands;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Initables;
using Cysharp.Threading.Tasks;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public partial class GUITab : IShowable, IInitableBase
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

        private readonly ReactiveProperty<bool> isShown = new(true);

        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_ShowOnInited;

        private bool isStateTransitioning;

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

        public virtual bool ShowAllowed => IsInited && !isStateTransitioning;
        public virtual bool HideAllowed => IsInited && !isStateTransitioning;
        public bool IsInited { get; private set; }

        private void IShowableAwake()
        {
            isShown.AddTo(this);
        }

        private void IShowableStart()
        {
            if (m_Graphic != null)
                UIHelper.DoTranpsarentRecursive(m_Graphic);

            this.DoActionAsync(Init);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IShowableOnTransformChildrenChanged()
        {
            Redraw();
        }

        private static async UniTask Init(GUITab @this)
        {
            await UniTask.NextFrame(timing: PlayerLoopTiming.PreUpdate);

            var childs = @this.Q()
                              .ByChildren()
                              .ExcludeSelf()
                              .Components<IInitableBase>()
                              .ZLinq()
                              .Where(x => x.Is<IShowable>())
                              .ToArray();

            if (childs.IsNotEmpty())
            {
                await UniTask.WaitUntil(
                    childs,
                    static childs => childs.All(x => x.IsInited),
                    timing: PlayerLoopTiming.PreUpdate
                    );
            }

            @this.IsInited = true;

            if (@this.m_Graphic != null)
            {
                var parents = @this.Q()
                                   .ByParent()
                                   .ExcludeSelf()
                                   .Components<IInitableBase>()
                                   .ZLinq()
                                   .Where(x => x.Is<IShowable>())
                                   .ToArray();

                await UniTask.WaitUntil(
                    parents,
                    static parents => parents.IsEmpty() || parents.All(x => x.IsInited),
                    timing: PlayerLoopTiming.PreUpdate
                    );

                UIHelper.UndoTransparentRecursive(@this.m_Graphic);
            }

            if (!@this.ShowOnInited && @this.GetParentGui().IsNone)
                @this.Hide();
        }

        public void Hide()
        {
            if (!IsShown)
                return;

            UIHelper.CaptureGraphicStatesUntilShowable(gameObject, graphicStates);
            UIHelper.CaptureShowableStatesUntilShowable(gameObject, showableStates);

            var command = Command.Create(this,
                isReadyToExecute: static @this => @this.HideAllowed,
                execute: static @this => @this.HideInternal(@this),
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
                execute: static @this => @this.ShowInternal(@this),
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
            //TODO:
        }

        public IObservable<Unit> ObserveShow()
        {
            return isShown.Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveHide()
        {
            return isShown.Where(x => !x).AsUnitObservable();
        }

        protected virtual void HideInternal<T>(T @this)
            where T : GUITab
        {
            @this.OnHide();

            foreach (var graphic in @this.graphicStates.ZLinq().Select(x => x.Target))
                @this.DisableGraphics(graphic);

            foreach (var showableState in @this.showableStates)
                showableState.Target.Hide();

            @this.OnHiden();
        }

        protected virtual void OnHide()
        {
            isStateTransitioning = true;
        }

        protected virtual void OnHiden()
        {
            isShown.Value = false;
            isStateTransitioning = false;
        }

        protected virtual void ShowInternal<T>(T @this)
            where T : GUITab
        {
            @this.OnShow();
            @this.graphicStates.RestoreStates();
            @this.graphicStates.Clear();
            @this.showableStates.RestoreStates();
            @this.showableStates.Clear();
            @this.OnShown();
        }

        protected virtual void OnShow()
        {
            isStateTransitioning = true;
        }

        protected virtual void OnShown()
        {
            isShown.Value = true;
            isStateTransitioning = false;
        }

        protected virtual void DisableGraphics(Graphic graphic)
        {
            if (graphic == null)
                return;

            graphic.enabled = false;
        }
    }
}
