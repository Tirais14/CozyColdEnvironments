using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using R3;
using TMPro;
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
        protected CompareAction<int> ShowCounterTextPredicate = new(1, CompareTypes.Equals | CompareTypes.Bigger);

        protected override void Init()
        {
            base.Init();

            if (viewModel.IsNotNull(out var vm))
                vm.ShowCounterTextPredicate = ShowCounterTextPredicate;

            BindItemIcon();
            BindItemCount();
        }

        private void BindItemIcon()
        {
            image.Maybe().IfSome(img =>
            {
                viewModelUnsafe.IconView.Subscribe(img,
                     static (sprite, img) => img.sprite = sprite)
                    .AddDisposableTo(this);
            });
        }

        private void BindItemCount()
        {
            counterMesh.IfSome(mesh =>
            {
                viewModelUnsafe.CounterView.Subscribe(mesh,
                    static (text, mesh) => mesh.text = text)
                    .AddDisposableTo(this);
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
