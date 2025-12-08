using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Snaphots.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem.HID;
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

        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_ShowOnInited;

        [NonSerialized]
        private Vector3 scaleBeforeInit;

        [NonSerialized]
        private Vector3 posBeforeInit;

        [NonSerialized]
        private bool stateTransitioning;

        public bool ShowOnInited {
            get => m_ShowOnInited;
            set => m_ShowOnInited = value;
        }

        public bool IsShown => isShown.Value;
        public virtual bool ShowAllowed => IsInited;
        public virtual bool HideAllowed => IsInited;
        public bool IsInited { get; private set; }

        [field: GetBySelf(IsOptional = true)]
        public CanvasGroup canvasGroup { get; private set; } = null!;

        private void IShowableAwake()
        {
            base.Awake();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            isShown.AddTo(this);

            scaleBeforeInit = transform.localScale;
            posBeforeInit = transform.localPosition;

            transform.localScale = Vector3.zero;
            transform.localPosition = PosOnInit;
        }

        private void IShowableStart()
        {
            base.Start();

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
                    @this.Q().FromParents().ExcludeSelf().Component<IShowable>().Lax().IsNone)
                {
                    @this.Hide();
                }

                @this.IsInited = true;
            });
        }

        public void Hide()
        {
            if (!IsShown || stateTransitioning)
                return;

            OnHide();
            DoHide();
            OnHiden();
        }

        public void Show()
        {
            if (IsShown || stateTransitioning)
                return;

            OnShow();
            DoShow();
            OnShown();
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
            DoRedraw();
        }

        public IObservable<Unit> ObserveShow()
        {
            return isShown.Where(static x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveHide()
        {
            return isShown.Where(static x => !x).AsUnitObservable();
        }

        protected virtual void OnHide()
        {
            stateTransitioning = true;
        }

        protected virtual void OnHiden()
        {
            stateTransitioning = false;
            isShown.Value = false;
        }

        protected virtual void DoHide()
        {
            snapshots.Add(new CanvasGroupSnapshot(canvasGroup));
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;

            foreach (var showable in this.Q()
                .FromChildrens()
                .FirstComponentsOnBranch()
                .Components<IShowable>())
            {
                snapshots.Add(new ShowableSnapshot(showable));
                showable.Hide();
            }
        }


        protected virtual void OnShow()
        {
            stateTransitioning = true;
        }

        protected virtual void OnShown()
        {
            stateTransitioning = false;
            isShown.Value = true;
        }

        protected virtual void DoShow()
        {
            int count = snapshots.Count;
            for (int i = 0; i < count; i++)
                snapshots[i].Restore();

            snapshots.Clear();
        }

        protected void DoRedraw()
        {
            var isShown = IsShown;

            Show();
            Hide();

            if (isShown)
                Show();
        }
    }
}
