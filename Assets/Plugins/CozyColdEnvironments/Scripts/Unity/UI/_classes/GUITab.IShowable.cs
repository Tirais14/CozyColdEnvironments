using CCEnvs.Patterns.Commands;
using CCEnvs.TypeMatching;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public partial class GUITab : IShowable
    {
        protected static Vector3 PosOnInit { get; } = new(10000f, 10000f);

        private readonly List<ISnapshot> snapshots = new();

        private readonly ReactiveProperty<bool> isShown = new(true);

        [NonSerialized]
        private bool redrawScheduled;

        [NonSerialized]
        private Vector3 scaleBeforeInit;

        [NonSerialized]
        private Vector3 posBeforeInit;

        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_ShowOnInited;

        public bool ShowOnInited {
            get => m_ShowOnInited;
            set => m_ShowOnInited = value;
        }

        public bool IsShown => isShown.Value;
        public virtual bool ShowAllowed => IsInited;
        public virtual bool HideAllowed => IsInited;
        public bool IsInited { get; private set; }

        private void IShowableAwake()
        {
            isShown.AddTo(this);

            scaleBeforeInit = transform.localScale;
            posBeforeInit = transform.localPosition;

            transform.localScale = Vector3.zero;
            transform.localPosition = PosOnInit;

        }

        private void IShowableStart()
        {
            this.DoActionAsync(static async (@this) =>
            {
                var childs = @this.Q()
                                  .ExcludeSelf()
                                  .FromChildrens()
                                  .Components<IShowable>()
                                  .Where(x => !x.IsInited)
                                  .ToArray();

                if (childs.IsNotEmpty())
                    await UniTask.WaitUntil(childs, static childs => childs.All(x => x.IsInited));

                @this.transform.localScale = @this.scaleBeforeInit;
                @this.transform.localPosition = @this.posBeforeInit;

                if (!@this.ShowOnInited
                    &&
                    @this.GetParentGUI().IsNone)
                {
                    @this.HideInternal();
                }

                @this.IsInited = true;
            });
        }

        private void IShowableOnTransformChildrenChanged()
        {
        }

        public void Hide()
        {
            if (!IsShown)
                return;

            HideInternal();
            //var command = Command.Create(this,
            //    isReadyToExecute: static @this => @this.HideAllowed,
            //    execute: static @this => @this.HideInternal(),
            //    name: nameof(Hide),
            //    undoCommandsOnAdd: Range.From(new CommandInfo(commandName: nameof(Show))),
            //    singleCommand: true
            //    );

            //commandScheduler.AddCommand(command);
        }

        public void Show()
        {
            if (IsShown)
                return;

            ShowInternal();
            //var command = Command.Create(this,
            //    isReadyToExecute: static @this => @this.ShowAllowed,
            //    execute: static @this => @this.ShowInternal(),
            //    name: nameof(Show),
            //    singleCommand: true
            //    );

            //commandScheduler.AddCommand(command);
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
            RedrawInternal();

            //var command = Command.Create(this,
            //    isReadyToExecute: @this => @this.IsInited,
            //    execute: static @this => @this.HideInternal(),
            //    name: nameof(Redraw),
            //    singleCommand: true
            //    );

            //commandScheduler.AddCommand(command);
        }

        public IObservable<Unit> ObserveShow()
        {
            return isShown.Where(static x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveHide()
        {
            return isShown.Where(static x => !x).AsUnitObservable();
        }

        protected void HideInternal()
        {
            OnHide();

            foreach (var x in snapshots.Select(static x => x.Target))
            {
                if (x.Is<Graphic>(out var graphic))
                    DisableGraphics(graphic);
                else if (x.Is<IShowable>(out var showable))
                    showable.Hide();
            }

            OnHiden();
        }

        protected virtual void OnHide()
        {
            UIHelper.CaptureGraphicStatesUntilShowable(gameObject, snapshots);
            UIHelper.CaptureShowableStatesUntilShowable(gameObject, snapshots);
        }

        protected virtual void OnHiden()
        {
            isShown.Value = false;
        }

        protected virtual void ShowInternal()
        {
            OnShow();

            int count = snapshots.Count;
            for (int i = 0; i < count; i++)
                snapshots[i].Restore();

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

        protected void RedrawInternal()
        {
            var isShown = IsShown;

            Show();
            Hide();

            if (isShown)
                Show();
        }
    }
}
