using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Commands;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Initables;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;
using ZLinq;
using static UnityEngine.Experimental.Rendering.GraphicsStateCollection;

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

        private bool redrawScheduled;

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
                       GetParentGui().Map(gui => gui.IsShown).GetValue(true);
            }
        }

        public virtual bool ShowAllowed => IsInited;
        public virtual bool HideAllowed => IsInited;
        public bool IsInited { get; private set; }

        private void IShowableStart()
        {
            if (m_Graphic != null)
                UIHelper.DoTransparent(m_Graphic);

            this.DoActionAsync(static async @this =>
            {
                await UniTask.NextFrame();

                var parentInitable = @this.Q()
                    .ByParent()
                    .ExcludeSelf()
                    .Component<IInitableBase>()
                    .Lax()
                    .Where(x => x.Is<IShowable>()
                    );

                await UniTask.WaitUntil(parentInitable,
                    static parentInitable => parentInitable.Map(x => x.IsInited).GetValue(true)
                    );

                @this.IsInited = true;

                if (@this.m_Graphic != null)
                    UIHelper.UndoTransparent(@this.m_Graphic);

                //if (!@this.ShowOnInited && @this.GetParentGui().IsNone)
                //    @this.Hide();
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IShowableOnTransformChildrenChanged()
        {
            Redraw();
        }

        public void Hide()
        {
            if (!IsShown)
                return;

            UIHelper.CaptureGraphicStates(gameObject, graphicStates);
            UIHelper.CaptureShowableStates(gameObject, showableStates);

            var command = Command.Create(
                this,
                static @this => @this.HideAllowed,
                static @this =>
                {
                    @this.HideInternal();
                },
                name: nameof(Hide)
                );

            commandScheduler.AddCommand(command);
        }

        public void Show()
        {
            if (IsShown)
                return;

            var command = Command.Create(
                this,
                static @this => @this.ShowAllowed,
                static @this =>
                {
                    @this.ShowInternal();
                },
                name: nameof(Show)
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
            if (redrawScheduled)
                return;

            //var command = Command.Create(
            //    this,
            //    static @this => @this.IsInited,
            //    static @this =>
            //    {
            //        @this.redrawScheduled = true;

            //        @this.DoActionAsync(@this,
            //            static async @this =>
            //            {
            //                await UniTask.NextFrame();

            //                Очистка старых состояний
            //                @this.graphicStates.Clear();
            //                @this.showableStates.Clear();

            //                Перепоказ текущего состояния
            //                if (@this.IsShown)
            //                {
            //                    Showable.Show(@this.gameObject, @this.graphicStates, @this.showableStates);
            //                }
            //                else
            //                {
            //                    Showable.Hide(@this.gameObject, @this.graphicStates, @this.showableStates);
            //                }

            //                @this.PrintLog($"{nameof(Redraw)} called.");
            //                @this.redrawScheduled = false;
            //            });
            //    },
            //    name: nameof(Redraw));

            //commandScheduler.AddCommand(command);
        }

        public IObservable<Unit> ObserveShow()
        {
            return isShown.Where(_ => IsInited).Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveHide()
        {
            return isShown.Where(_ => IsInited).Where(x => !x).AsUnitObservable();
        }

        protected virtual void HideInternal()
        {
            foreach (var graphicState in graphicStates)
                graphicState.Target.enabled = false;

            foreach (var showableState in showableStates)
                showableState.Target.Hide();

            isShown.Value = false;
        }

        protected virtual void ShowInternal()
        {
            graphicStates.RestoreStates();
            showableStates.RestoreStates();
            isShown.Value = true;
        }
    }
}
