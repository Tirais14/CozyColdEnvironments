using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Language;
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
        private Maybe<Component> _dragItem;

        [field: SerializeField, GetByChildren]
        protected Maybe<TextMeshProUGUI> textMesh { get; private set; } = null!;

        protected override Maybe<Component> dragItem => _dragItem;
        protected override bool ShowOnStart => true;
        protected override bool readyToDrag => model.Contains();
        protected override bool readyToTakeDrop => !model.IsFull;

        protected override void Start()
        {
            base.Start();

            image.IfSome((System.Action<Image>)(img =>
            {
                ObservableExtensions.Subscribe<Sprite>(base.viewModel.ItemIconView, (System.Action<Sprite>)(newSprite => img.sprite = newSprite))
                                      .AddTo(this);
            }));

            textMesh.IfSome((System.Action<TextMeshProUGUI>)((mesh) =>
            {
                Observable.Select<int, string>(base.viewModel.ItemCountView, (System.Func<int, string>)(y => y.ToString()))
                                       .Subscribe(newText => mesh.text = newText)
                                       .AddTo(this);
            }));
        }

        protected override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            if (!readyToDrag)
                return;

            _dragItem = Instantiate(this, transform);
        }

        protected override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            if (!readyToDrag)
                return;

            dragItem.Match(
                x => Destroy(x.gameObject),
                () => this.PrintWarning($"{nameof(dragItem)} is null. This is unexpected behaviour for DragAndDrop system, but not critically. May do cause of memory leak.")
                );
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
