using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UTIRLib.Attributes;

#pragma warning disable IDE0044
#nullable enable
namespace UTIRLib.UI.ItemSystem
{
    [RequireComponent(typeof(Image))]
    public class ItemStackUIView : View<IItemStackUIViewModel>, IMovable
    {
        private Vector2 defaultLocalPosition;

        [GetBySelf]
        private Image image = null!;

        [Optional]
        [GetByChildren]
        [SerializeField]
        private TextView? textComponent;

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
                viewModel.ItemCount.Subscribe(x => textComponent.Text = x).AddTo(this);
        }

        private void OnDestroy() => viewModel.Dispose();

        void IMovable.ResetPosition()
        {
            transform.localPosition = defaultLocalPosition;
        }
    }
}
