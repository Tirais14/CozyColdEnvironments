using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable S1125
namespace CCEnvs.Unity.Storages.UI
{
    [RequireComponent(typeof(Image))]
    public abstract class ItemContainerView<TViewModel>
        : View<TViewModel>

        where TViewModel : IItemContainerViewModel
    {
        [SerializeField]
        [GetByChildren(IsOptional = true)]
        protected Maybe<TextMeshProUGUI> counterMesh;

        [SerializeField]
        protected CompareAction<int> ShowCounterTextPredicate = new(1, CompareTypes.Bigger);

        protected override void Awake()
        {
            base.Awake();
            isMutableView = true;

            image.Raw.ObserveEveryValueChanged(cmp => cmp!.enabled).Subscribe(state => 
            this.PrintLog(state)
            );
        }

        protected override void Start()
        {
            base.Start();
            viewModelUnsafe.ShowCounterTextPredicate = ShowCounterTextPredicate;
        }

        protected override void Init()
        {
            base.Init();
            BindItemIcon();
            BindItemCount();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SelectableDoSelectPredicate()
        {
            return base.SelectableDoSelectPredicate()
                   &&
                   viewModel.Map(vm => vm.model)
                            .Cast<IItemContainer>()
                            .Match(Right: cnt => !cnt.IsEmpty,
                                   Left: _ => false,
                                   Other: () => false
                                   );
        }

        private void BindItemIcon()
        {
            image.IfSome(img =>
            {
                viewModelUnsafe.IconView.SubscribeWithState(img,
                        static (sprite, img) => img.sprite = sprite)
                    .AddTo(this);
            });
        }

        private void BindItemCount()
        {
            counterMesh.IfSome(mesh =>
            {
                viewModelUnsafe.CounterView.SubscribeWithState(mesh,
                        static (text, mesh) => mesh.text = text)
                    .AddTo(this);
            });
        }
    }
    public class ItemContainerView : ItemContainerView<ItemContainerViewModel<IItemContainer>>
    {
        protected override Maybe<ItemContainerViewModel<IItemContainer>> ViewModelFactory()
        {
            var cnt = new ItemContainer();
            return new ItemContainerViewModel<IItemContainer>(cnt);
        }
    }
}
