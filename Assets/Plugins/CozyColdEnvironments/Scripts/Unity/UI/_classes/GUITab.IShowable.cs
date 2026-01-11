using CCEnvs.Snapshots;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Snapshots.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using ZLinq;
using CCEnvs.Unity.Components;
using CCEnvs.Collections;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public partial class GUITab : IShowable
    {
        private readonly Dictionary<object, ISnapshot> snapshots = new();
        private readonly ReactiveProperty<bool> isShown = new(true);

        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_ShowOnInited;

        [SerializeField]
        protected ShowableSettings showableSettings = ShowableSettings.Default;

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
            if (!showableSettings.IsFlagSetted(ShowableSettings.ShowHideByGameObjectState)
                &&
                canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            isShown.AddToBehaviour(this);
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

                if (!@this.ShowOnInited
                    &&
                    @this.Q().FromParents().ExcludeSelf().Component<IShowable>().Lax().IsNone)
                {
                    @this.Hide();
                }

                @this.IsInited = true;
            });
        }

        public virtual void Hide()
        {
            if (!IsShown || stateTransitioning)
                return;

            OnHide();
            DoHide();
            OnHiden();
        }

        public virtual void Show()
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
        }

        protected virtual void OnHiden()
        {
            stateTransitioning = false;
            isShown.Value = false;
        }

        protected virtual void DoHide()
        {
            if (showableSettings.IsFlagSetted(ShowableSettings.ShowHideByGameObjectState))
            {
                gameObject.SetActive(false);
                return;
            }

            snapshots.Add(canvasGroup, new CanvasGroupSnapshot(canvasGroup));
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            foreach (var showable in this.Q()
                .FromChildrens()
                .FirstComponentsOnBranch()
                .Components<IShowable>())
            {
                snapshots.Add(showable, new ShowableSnapshot(showable));
                showable.Hide();
            }
        }


        protected virtual void OnShow()
        {
            stateTransitioning = true;

            if (showableSettings.IsFlagSetted(ShowableSettings.ShowHideByGameObjectState))
                gameObject.SetActive(true);
        }

        protected virtual void OnShown()
        {
            stateTransitioning = false;
            isShown.Value = true;
        }

        protected virtual void DoShow()
        {
            using var snapshotsCopyHandle = snapshots.ToArrayPooled();

            foreach (var pair in snapshotsCopyHandle.Value)
                pair.Value.TryRestore(pair.Key, out _);
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
