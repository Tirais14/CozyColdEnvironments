using Cysharp.Threading.Tasks;
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
    public class ItemStackUIView : MonoX
    {
        [GetComponent]
        private Image image = null!;

        [Optional]
        [SerializeField]
        [GetComponentInChildrenIfNull]
        private TextMeshProUGUI? textComponent;

        [GetComponent]
        private ItemStackUIViewModel viewModel = null!;

        protected override void OnStart()
        {
            base.OnStart();

            _ = BindAsync();
        }

        private async UniTaskVoid BindAsync()
        {
            await UniTask.WaitUntil(() => viewModel.didStart);

            viewModel.IconView.Subscribe(x => image.sprite = x);

            if (textComponent != null)
                viewModel.CountView.Subscribe(x => textComponent.text = x);
        }
    }
}
