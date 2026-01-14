using CCEnvs.Collections;
using CCEnvs.Patterns.Commands;
using CCEnvs.Snapshots;
using CCEnvs.Unity.Snapshots.UI;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public partial class GUITab : IShowable
    {
        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_ShowOnInited;

        [SerializeField]
        protected ShowableRenderMode showableRenderMode;

        [NonSerialized]
        private readonly ReactiveProperty<bool> isShown = new(true);

        [NonSerialized]
        private readonly Lazy<Dictionary<object, ISnapshot>> snapshots = new(() => new Dictionary<object, ISnapshot>());

        [NonSerialized]
        private bool stateTransitioning;

        private ICommand hideCmd = null!;
        private ICommand onHidenCmd = null!;
        private ICommand showCmd = null!;
        private ICommand onShownCmd = null!;

        public bool ShowOnInited {
            get => m_ShowOnInited;
            set => m_ShowOnInited = value;
        }

        public bool IsShown => isShown.Value;
        public virtual bool ShowAllowed => IsInited && !stateTransitioning;
        public virtual bool HideAllowed => IsInited && !stateTransitioning;
        public bool IsInited { get; private set; }

        private void IShowableAwake()
        {
            if (canvasGroup.IsNone
                &&
                showableRenderMode == ShowableRenderMode.CanvasGroup)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            CreateShowCommands();
            CreateHideCommands();
        }

        private void IShowableStart()
        {
            UniTask.Create(this,
                static async @this =>
                {
                    await @this.WaitUntilChildrensInitedAsync();

                    @this.InitVisibleState();

                    @this.RedrawCanvas();

                    @this.IsInited = true;
                })
                .Forget();
        }

        private void IShowableOnDestroy()
        {
            isShown.Dispose();
        }

        public virtual void Hide()
        {
            commandScheduler.Schedule(hideCmd.Reset());
            commandScheduler.Schedule(onHidenCmd.Reset());
        }

        public virtual void Show()
        {
            commandScheduler.Schedule(showCmd.Reset());
            commandScheduler.Schedule(onShownCmd.Reset());
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

        protected virtual void DoHide()
        {
            switch (showableRenderMode)
            {
                case ShowableRenderMode.GameObject:
                    {
                        gameObject.SetActive(false);
                        SetHiden();
                    }
                    break;
                case ShowableRenderMode.CanvasGroup:
                    {
                        var canvasGroup = this.canvasGroup.GetValueUnsafe(static () => throw new InvalidOperationException("Canvas group not found."));

                        snapshots.Value.Add(canvasGroup, new CanvasGroupSnapshot(canvasGroup));

                        canvasGroup.alpha = 0f;
                        canvasGroup.blocksRaycasts = false;
                        canvasGroup.interactable = false;

                        foreach (var showable in this.Q()
                            .FromChildrens()
                            .FirstComponentsOnBranch()
                            .Components<IShowable>())
                        {
                            snapshots.Value.Add(showable, new ShowableSnapshot(showable));
                            showable.Hide();
                        }

                        UniTask.Create(this,
                            static async @this =>
                            {
                                await UniTask.NextFrame(
                                    timing: PlayerLoopTiming.Initialization,
                                    cancellationToken: @this.destroyCancellationToken
                                    );

                                @this.gameObject.SetActive(false);
                                @this.SetHiden();
                            })
                            .Forget();
                    }
                    break;
                default:
                    break;
            }
        }

        protected virtual void OnHiden()
        {
        }

        protected virtual void OnShow()
        {
            stateTransitioning = true;
        }

        protected virtual void DoShow()
        {
            switch (showableRenderMode)
            {
                case ShowableRenderMode.GameObject:
                    {
                        gameObject.SetActive(true);
                        SetShown();
                    }
                    break;
                case ShowableRenderMode.CanvasGroup:
                    {
                        using var snapshotsCopyHandle = snapshots.Value.ToArrayPooled();

                        foreach (var pair in snapshotsCopyHandle.Value)
                        {
                            pair.Value.TryRestore(pair.Key, out _);
                            snapshots.Value.Remove(pair.Key);
                        }

                        gameObject.SetActive(true);
                        SetShown();
                    }
                    break;
                default:
                    break;
            }
        }

        protected virtual void OnShown()
        {
        }

        protected void DoRedraw()
        {
            var isShown = IsShown;

            Show();
            Hide();

            if (isShown)
                Show();
        }

        private void SetShown()
        {
            stateTransitioning = false;
            isShown.Value = true;

            OnShown();
        }

        private void SetHiden()
        {
            stateTransitioning = false;
            isShown.Value = false;

            OnHiden();
        }

        private void InitVisibleState()
        {
            if (!ShowOnInited
                &&
                this.Q()
                .FromParents()
                .ExcludeSelf()
                .IncludeInactive()
                .Component<IShowable>()
                .Lax()
                .IsNone)
            {
                Hide();
            }
        }

        private async UniTask WaitUntilChildrensInitedAsync()
        {
            IShowable[] childs = this.Q()
                .FromChildrens()
                .ExcludeSelf()
                .IncludeInactive()
                .Components<IShowable>()
                .Where(x => !x.IsInited)
                .ToArray();

            if (ArrayHelper.IsNotEmpty(childs))
            {
                await UniTask.WaitUntil(
                    childs,
                    childs => childs.All(x => x.IsInited),
                    cancellationToken: destroyCancellationToken
                    );
            }
        }

        private void CreateHideCommands()
        {
            hideCmd = Command.Builder.OnExecute(this,
                static @this =>
                {
                    @this.OnHide();
                    @this.DoHide();
                })
                .ExecuteWhen(this,
                static @this =>
                {
                    return @this.HideAllowed;
                })
                .Build();

            onHidenCmd = Command.Builder.OnExecute(this,
                static @this =>
                {
                    @this.OnHiden();
                })
                .ExecuteWhen(this,
                static @this =>
                {
                    return @this.HideAllowed;
                })
                .Build();
        }

        private void CreateShowCommands()
        {
            showCmd = Command.Builder.OnExecute(this,
                static @this =>
                {
                    @this.OnShow();
                    @this.DoShow();
                })
                .ExecuteWhen(this,
                static @this =>
                {
                    return @this.ShowAllowed;
                })
                .Build();

            onShownCmd = Command.Builder.OnExecute(this,
                static @this =>
                {
                    @this.OnShown();
                })
                .ExecuteWhen(this,
                static @this =>
                {
                    return @this.ShowAllowed;
                })
                .Build();
        }

        private void RedrawCanvas()
        {
            //if (canvas.enabled)
            //{
            //    var goState = gameObject.activeSelf;

            //    canvas.enabled = false;

            //    gameObject.SetActive(true);

            //    canvas.enabled = true;

            //    gameObject.SetActive(goState);
            //}

            //UniTask.Create(this,
            //    static async @this =>
            //    {
            //        await UniTask.DelayFrame(2);
            //        LayoutRebuilder.ForceRebuildLayoutImmediate(@this.canvas.RectTransform());
            //    })
            //    .Forget();

        }
    }
}
