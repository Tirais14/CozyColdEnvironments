using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;
using TMPro;
using R3;
using UnityEngine;
using UnityEngine.UI;
using CCEnvs.Unity.Components;

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
        protected CompareAction<int> ShowCounterTextPredicate = new(1, CompareTypes.Equals | CompareTypes.Bigger);

        protected override void Init()
        {
            base.Init();

            if (viewModel.TryGetValue(out var vm))
                vm.ShowCounterTextPredicate = ShowCounterTextPredicate;

            BindItemIcon();
            BindItemCount();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SelectableDoSelectPredicate()
        {
            return base.SelectableDoSelectPredicate()
                   &&
                   viewModel.Map(vm => vm.model).Cast<IItemContainer>().TryGetRightValue(out var cnt)
                   &&
                   cnt.ContainsItem();
        }

        private void BindItemIcon()
        {
            image.IfSome(img =>
            {
                viewModelUnsafe.IconView.Subscribe(img,
                     static (sprite, img) => img.sprite = sprite)
                    .RegisterDisposableTo(this);
            });
        }

        private void BindItemCount()
        {
            counterMesh.IfSome(mesh =>
            {
                viewModelUnsafe.CounterView.Subscribe(mesh,
                    static (text, mesh) => mesh.text = text)
                    .RegisterDisposableTo(this);
            });
        }
    }
    public class ItemContainerView : ItemContainerView<ItemContainerViewModel<IItemContainer>>
    {
        protected override Maybe<ItemContainerViewModel<IItemContainer>> CreateViewModel()
        {
            var cnt = new ItemContainer();
            return new ItemContainerViewModel<IItemContainer>(cnt, destroyCancellationToken);
        }
    }
}
