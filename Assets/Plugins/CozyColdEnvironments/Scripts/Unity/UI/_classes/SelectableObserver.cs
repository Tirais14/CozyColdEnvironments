using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using ZLinq;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.UI
{
    [DisallowMultipleComponent]
    public class SelectableObserver<T> : CCBehaviour, ISelectableController<T>
        where T : ISelectable
    {
        protected readonly ReactiveProperty<Maybe<T>> selection = new();
        protected readonly C5.HashSet<T> selectables = new();
        private CompositeDisposable disposables = new();
        private bool collectSelectablesScheduled;

        public Maybe<T> Selection => selection.Value;

        protected override void Awake()
        {
            base.Awake();
            selectables.CollectionCleared += OnSelectablesClear;
        }

        protected override void Start()
        {
            base.Start();
            InitSelectables();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            InitSelectables();
        }

        protected virtual void OnDestroy()
        {
            selectables.CollectionCleared -= OnSelectablesClear;
            disposables.Dispose();
        }

        protected static void OnSeleactableAdd<TValue>(SelectableObserver<T> inst, T cmp)
        {
            cmp.ObserveDoSelect()
                .Merge(cmp.ObserveDoDeselect())
                .SubscribeWithState(inst,
                static (slct, inst) => inst.selection.Value = slct.As<T>())
                .AddTo(inst.disposables);
        }

        public IObservable<T> ObserveDeselected()
        {
            return selection.Pairwise()
                            .Where(pair => pair.Current.IsNone)
                            .Select(pair => pair.Previous.GetValueUnsafe());
        }

        public IObservable<T> ObserveSelected()
        {
            return selection.Where(x => x.IsSome).Select(x => x.GetValueUnsafe());
        }

        public IObservable<PreviousCurrentPair<Maybe<T>>> ObserveSelection()
        {
            return selection.Pairwise().Select(pair => PreviousCurrentPair.Create(pair.Previous, pair.Current));
        }

        protected virtual void CollectSelectables()
        {
            foreach (var cmp in this.QueryTo()
                                    .ByChildren()
                                    .ExcludeSelf()
                                    .Models<T>())
            {
                selectables.Add(cmp);
                OnSeleactableAdd<T>(this, cmp);
            }
        }

        protected void InitSelectables()
        {
            if (collectSelectablesScheduled)
                return;

            selectables.Clear();
            selection.Value = Maybe<T>.None;
            collectSelectablesScheduled = true;

            this.DoActionAsync(static async @this =>
            {
                await UniTask.NextFrame(PlayerLoopTiming.LastInitialization);
                try
                {
                    @this.CollectSelectables();
                }
                catch (Exception ex)
                {
                    @this.PrintError(ex);
                }
                finally
                {
                    @this.collectSelectablesScheduled = false;
                }
            });
        }

        private void OnSelectablesClear(object sender, EventArgs args)
        {
            disposables.Dispose();
            disposables = new CompositeDisposable();
        }
    }
    public class SelectableObserver : SelectableObserver<ISelectable>
    {
    }
}
