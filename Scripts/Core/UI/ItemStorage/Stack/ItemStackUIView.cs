using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UTIRLib.Attributes;
using UTIRLib.ComponentSetter;

#nullable enable
namespace UTIRLib.UI
{
    [RequireComponent(typeof(Image))]
    public class ItemStackUIView : View, IView<IItemStackUIViewModel>, IMovable
    {
        private Vector2 defaultLocalPosition;

        [GetSelfAttribute]
        private Image image = null!;

        [Optional]
        [GetByChildren]
        [SerializeField]
        private TextMeshProUGUI? textComponent;

        private IItemStackUIViewModel viewModel = null!;

        IItemStackUIViewModel IView<IItemStackUIViewModel>.ViewModel => viewModel;
        Vector2 IMovable.Position {
            get => transform.position;
            set => transform.position = value;
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            var itemStack = new ItemStackUIReactive();
            viewModel = new ItemStackUIViewModel(itemStack);
        }

        protected override void OnStart()
        {
            base.OnStart();

            defaultLocalPosition = transform.localPosition;

            viewModel.ItemIcon.Subscribe(x => image.sprite = x).AddTo(this);

            if (textComponent != null)
                viewModel.ItemCount.Subscribe(x => textComponent.text = x).AddTo(this);
        }

        private void OnDestroy() => viewModel.Dispose();

        void IMovable.ResetPosition()
        {
            transform.localPosition = defaultLocalPosition;
        }
    }
}
