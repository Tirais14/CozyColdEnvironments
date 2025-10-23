using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Language;
using CCEnvs.UI.MVVM;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.UI.MVVM;
using Cysharp.Threading.Tasks;
using System.Linq;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZLinq;


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
        private Ghost<Component> _dragItem;

        [field: SerializeField, GetByChildren]
        protected Ghost<TextMeshProUGUI> textMesh { get; private set; } = null!;

        protected override Ghost<Component> dragItem => _dragItem;
        protected override bool ShowOnStart => true;
        protected override bool readyToDrag => model.Contains();
        protected override bool readyToTakeDrop => !model.IsFull;

        protected override void Start()
        {
            base.Start();

            image.IfSome(img =>
            {
                viewModel.ItemIconView.Subscribe(newSprite => img.sprite = newSprite)
                                      .AddTo(this);
            });

            textMesh.IfSome((mesh) =>
            {
                viewModel.ItemCountView.Select(y => y.ToString())
                                       .Subscribe(newText => mesh.text = newText)
                                       .AddTo(this);
            });
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

            eventData.selectedObject.ToGhost()
                                    .Map(x => x.GetAssignedModel<IItemContainer>()!)
                                    .IfSome(x => x.Put(model));
        }
    }
    public class ItemContainerView : ItemContainerView<ItemContainerViewModel<ItemContainer>, ItemContainer>
    {
    }
}
