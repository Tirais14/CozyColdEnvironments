using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using CCEnvs.Unity.UI.MVVM;
using Cysharp.Threading.Tasks;
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
    public abstract class ItemContainerView<TViewModel, TContainer>
        : View<TViewModel, TContainer>,
        IItemContainerView<TViewModel, TContainer>

        where TViewModel : ViewModel<TContainer>, IItemContainerViewModel<TContainer>
        where TContainer : IItemContainer, new()
    {
        [SerializeField]
        [GetByChildren(IsOptional = true)]
        protected Maybe<TextMeshProUGUI> counterMesh;

        public CompareAction<int> ShowCounterTextPredicate = new(1, CompareTypes.Bigger);

        protected override void Awake()
        {
            base.Awake();
            ShowableSettings |= IShowable.Settings.KeepRaycastTargetState;
            ShowableSettings &= ~IShowable.Settings.ByComponentState;
        }

        protected override void Start()
        {
            base.Start();
            viewModel.ShowCounterTextPredicate = ShowCounterTextPredicate;
        }

        protected override void InstallBingings()
        {
            base.InstallBingings();
            BindItemIcon();
            BindItemCount();
        }

        private void BindItemIcon()
        {
            image.IfSome(img =>
            {
                viewModel.ItemView.SubscribeWithState(img,
                        static (sprite, img) => img.sprite = sprite)
                    .AddTo(this);
            });
        }

        private void BindItemCount()
        {
            counterMesh.IfSome(mesh =>
            {
                viewModel.CounterText.SubscribeWithState(mesh,
                        static (text, mesh) => mesh.text = text)
                    .AddTo(this);
            });
        }
    }
    public class ItemContainerView : ItemContainerView<ItemContainerViewModel<ItemContainer>, ItemContainer>
    {
    }
}
