using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System;
using System.Linq;
using R3;
using UnityEngine;
using ZLinq;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.UI
{
    [DisallowMultipleComponent]
    public class SelectableController<T> : CCBehaviour, ISelectableController<T>
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            selectables.CollectionCleared -= OnSelectablesClear;
            disposables.Dispose();
        }

        protected static void OnSeleactableAdd<TValue>(SelectableController<T> inst, T cmp)
        {
            cmp.ObserveDoSelect()
               .Subscribe(inst,
               static (slct, inst) =>
               {
                   inst.selection.Value.IfSome(x => x.DoDeselect());
                   inst.selection.Value = slct.AsMaybe<T>();
               })
               .AddTo(inst.disposables);

            cmp.ObserveDoDeselect()
               .Subscribe(inst,
               static (_, inst) =>
               {
                   inst.selection.Value.IfSome(x => x.DoDeselect());
                   inst.selection.Value = Maybe<T>.None;
               })
               .AddTo(inst.disposables);
        }

        public void ResetSelection()
        {
            selection.Value.IfSome(x => x.DoDeselect());
        }

        public Observable<T> ObserveDeselected()
        {
            return selection.Pairwise()
                            .Where(pair => pair.Current.IsNone)
                            .Select(pair => pair.Previous.GetValueUnsafe());
        }

        public Observable<T> ObserveSelected()
        {
            return selection.Where(x => x.IsSome).Select(x => x.GetValueUnsafe());
        }

        public Observable<Maybe<T>> ObserveSelection()
        {
            return selection;
        }

        protected virtual void CollectSelectables()
        {
            foreach (var cmp in this.QueryTo()
                                    .FromChildrens()
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

            UniTask.Create(this,
                static async @this =>
                {
                    await UniTask.NextFrame(PlayerLoopTiming.LastInitialization);

                    try
                    {
                        @this.CollectSelectables();
                    }
                    finally
                    {
                        @this.collectSelectablesScheduled = false;
                    }
                })
                .Forget();
        }

        private void OnSelectablesClear(object sender, EventArgs args)
        {
            disposables.Dispose();
            disposables = new CompositeDisposable();
        }
    }
    public class SelectableObserver : SelectableController<ISelectable>
    {
    }
}
