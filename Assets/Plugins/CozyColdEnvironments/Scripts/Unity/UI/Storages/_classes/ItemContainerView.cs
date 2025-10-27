using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.UI.MVVM;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.UI.MVVM;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


#nullable enable
#pragma warning disable IDE0044
#pragma warning disable IDE1006
#pragma warning disable IDE0051
#pragma warning disable S3236
namespace CCEnvs.Unity.UI.Storages
{
    [RequireComponent(typeof(Image))]
    public abstract class ItemContainerView<TViewModel, TContainer>
        : View<TViewModel, TContainer>,
        IItemContainerView<TViewModel, TContainer>

        where TViewModel : ViewModel<TContainer>, IItemContainerViewModel<TContainer>
        where TContainer : IItemContainer, new()
    {
        [field: SerializeField, GetByChildren(IsOptional = true)]
        protected Maybe<TextMeshProUGUI> textMesh { get; private set; } = null!;

        protected override bool ShowOnStart => true;
        protected override (bool state, Maybe<string> msg) readyToDrag {
            get
            {
                if (model.IsEmpty)
                    return (false, $"Cannot start dragging. {nameof(ItemContainer)} is empty. {nameof(ItemContainer)}: {model}");

                return (true, null);
            }
        }
        protected override bool readyToTakeDrop => !model.IsFull;
        protected override bool resetPositionAfterDrag => true;
        protected override bool dragCopyOfThis => true;

        protected override void Start()
        {
            base.Start();

            textMesh = GetComponentInChildren<TextMeshProUGUI>();

            image.IfSome(img =>
            {
                ObservableExtensions.Subscribe(viewModel.ItemIconView, newSprite => img.sprite = (Sprite?)newSprite)
                                    .AddTo(this);
            });

            textMesh.IfSome(mesh =>
            {
                Observable.Select(viewModel.ItemCountView, y => y.ToString())
                          .Subscribe(newText => mesh.text = newText)
                          .AddTo(this);
            });
        }

        protected override void OnDrop(PointerEventData eventData)
        {
            if (!readyToTakeDrop)
                return;

            eventData.selectedObject.Maybe()
                                    .Map(x => x.GetAssignedModel<IItemContainer>()!)
                                    .IfSome(x => x.Put(model));
        }
    }
    public class ItemContainerView : ItemContainerView<ItemContainerViewModel<ItemContainer>, ItemContainer>
    {
    }
}
