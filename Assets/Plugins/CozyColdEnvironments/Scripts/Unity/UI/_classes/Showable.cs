using CCEnvs.Snapshots;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Snapshots.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.UI;
using CCEnvs.Collections;

#nullable enable
namespace CCEnvs.Unity.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    public class Showable : CCBehaviour, IShowable
    {
        protected static Vector3 PosOnInit { get; } = new(10000f, 10000f);

        private readonly List<ISnapshot> snapshots = new();
        private readonly ReactiveProperty<bool> isShown = new(true);

        [NonSerialized]
        private Vector3 scaleBeforeInit;

        [NonSerialized]
        private Vector3 posBeforeInit;

        [NonSerialized]
        private bool stateTransitioning;

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

        [field: GetBySelf]
        public Graphic graphic { get; private set; } = null!;

        [field: GetBySelf(IsOptional = true)]
        public CanvasGroup canvasGroup { get; private set; } = null!;

        protected override void Awake()
        {
            base.Awake();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            isShown.BindDisposableTo(this);

            scaleBeforeInit = transform.localScale;
            posBeforeInit = transform.localPosition;

            transform.localScale = Vector3.zero;
            transform.localPosition = PosOnInit;
        }

        protected override void Start()
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

        public Observable<Unit> ObserveShow()
        {
            return isShown.Where(static x => x).AsUnitObservable();
        }

        public Observable<Unit> ObserveHide()
        {
            return isShown.Where(static x => !x).AsUnitObservable();
        }

        protected virtual void OnHide()
        {
            stateTransitioning = true;
            CaptureSnapshots();
        }

        protected virtual void OnHiden()
        {
            stateTransitioning = false;
            isShown.Value = false;
        }

        protected virtual void DoHide(Transform? target = null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
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

        protected virtual void CaptureSnapshots()
        {
            snapshots.Add(new CanvasGroupSnapshot(canvasGroup));
        }
    }
}
