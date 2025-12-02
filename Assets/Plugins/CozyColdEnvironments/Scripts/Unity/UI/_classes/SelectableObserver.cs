using CCEnvs.Diagnostics;
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

        protected static void FindSelected(Unit _, SelectableObserver<T> @this)
        {
            @this.selection.Value.IfSome(x => x.DoDeselect());

            @this.selection.Value = @this.selectables.ZLinq()
                .Where(sel => sel.IsNotNull())
                .FirstOrDefault(sel => sel.IsSelected);
        }

        protected static void OnAdd<TValue>(SelectableObserver<T> @this, T cmp)
        {
            cmp.ObserveDoSelect().SubscribeWithState(@this, FindSelected).AddTo(@this.disposables);
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
            foreach (var cmp in this.QueryTo().ByChildren().ExcludeSelf().Models<T>())
            {
                selectables.Add(cmp);
                OnAdd<T>(this, cmp);
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
                await UniTask.NextFrame();
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
                    await UniTask.WaitForEndOfFrame();
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
