using CCEnvs.Reflection.Data;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.UI.MVVM;
using UniRx;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI.Storages
{
    [RequireComponent(typeof(Image))]
    public abstract class ItemContainerView<TViewModel, TContainer> : AView<TViewModel>
        where TViewModel : AViewModel<TContainer>, IItemContainerViewModel
        where TContainer : IItemContainer, new()
    {
        [GetBySelf]
        protected Image image { get; private set; } = null!;

        [GetByChildren]
        [field: SerializeField]
        protected TextMeshProUGUI textMesh { get; private set; } = null!;

        protected override void Start()
        {
            base.Start();

            viewModel.ItemIconView.Subscribe(x => image.sprite = x)
                                  .AddTo(this);

            viewModel.ItemCountView.Select(x => x.ToString())
                                   .Subscribe(x => textMesh.text = x)
                                   .AddTo(this);
        }

        protected override TViewModel CreateViewModel()
        {
            return InstanceFactory.Create<TViewModel>(
                new ExplicitArguments(new ExplicitArgument(new TContainer())),
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);
        }
    }
}
