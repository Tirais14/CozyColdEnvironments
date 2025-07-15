using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UTIRLib.Attributes;
using UTIRLib.Injector;

#nullable enable
namespace UTIRLib.UI
{
    [RequireComponent(typeof(Image), typeof(ItemStackUIViewModel))]
    public class ItemStackUIView : View, IView<IItemStackUIViewModel>
    {
        [GetComponent]
        private Image image = null!;

        [Optional]
        [SerializeField]
        [GetComponentInChildrenIfNull]
        private TextMeshProUGUI? textComponent;

        private IItemStackUIViewModel viewModel = null!;

        IItemStackUIViewModel IView<IItemStackUIViewModel>.ViewModel => viewModel;

        protected override void OnAwake()
        {
            base.OnAwake();

            var itemStack = new ItemStackUIReactive();
            viewModel = new ItemStackUIViewModel(itemStack);
        }

        protected override void OnStart()
        {
            base.OnStart();

            viewModel.ItemIcon.Subscribe(x => image.sprite = x).AddTo(this);

            if (textComponent != null)
                viewModel.ItemCount.Subscribe(x => textComponent.text = x).AddTo(this);
        }

        private void OnDestroy() => viewModel.Dispose();
    }
}
