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

        protected override bool showOnStart => true;

        protected override void Awake()
        {
            base.Awake();

            dragSettings = DragAndDropSettings.ResetPos;
        }

        protected override void Start()
        {
            base.Start();

            textMesh = GetComponentInChildren<TextMeshProUGUI>();

            ObservableExtensions.Subscribe(viewModel.ItemIconView, newSprite => Img.AccessUnsafe().sprite = (Sprite?)newSprite)
                                .AddTo(this);

            textMesh.IfSome(mesh =>
            {
                Observable.Select(viewModel.ItemCountView, y => y.ToString())
                          .Subscribe(newText => mesh.text = newText)
                          .AddTo(this);
            });
        }

        protected override bool DragPredicate(out Maybe<string> msg)
        {
            if (model.IsEmpty)
            {
                msg = $" Dragging is not possible. {nameof(ItemContainer)} is empty.";
                return false;
            }

            msg = null;
            return true;
        }

        protected override void OnDrop(PointerEventData eventData)
        {
            if (DropPredicate())
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
