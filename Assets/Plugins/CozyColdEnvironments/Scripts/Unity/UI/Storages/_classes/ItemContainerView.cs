using CCEnvs.Diagnostics;
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
        protected Maybe<TextMeshProUGUI> counterMesh { get; private set; } = null!;

        protected override bool showOnStart => true;

        protected override void Awake()
        {
            base.Awake();

            dragSettings = DragAndDropSettings.ResetPos
                           |
                           DragAndDropSettings.RefillEmptySpace
                           |
                           DragAndDropSettings.SetAsLastSiblingWhenDragging
                           |
                           DragAndDropSettings.InHighPriorityCanvas;

            showableDisableGraphicsSettings = DisableGraphicsSettings.KeepRaycastTargetState;
        }

        protected override void Start()
        {
            base.Start();

            BindItemIcon();
            BindItemCount();
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
            if (
               !DropPredicate()
                || 
                eventData.pointerDrag == cGameObject.Value
                )
                return;

            eventData.pointerDrag.Maybe()
                                 .Map(go => go.GetAssignedModel<IItemContainer>().Access())
                                 .Map(cnt => (source: cnt, rest: model.Put(cnt)))
                                 .Where(cnt => cnt.rest.IsSome)
                                 .IfSome(cnt => cnt.source.Put(cnt.rest.AccessUnsafe()));

        }

        private void BindItemIcon()
        {
            Img.IfSome(x =>
            {
                x.sprite = viewModel.ItemIconView.Value;

                viewModel.ItemIconVisible.Value.Resolve()
                                               .If(Show)
                                               .Else(() => Hide());
            });
        }

        private void BindItemCount()
        {
            counterMesh.IfSome(x =>
            {
                viewModel.ItemCountView.Subscribe(
                    text => x.text = text)
                                       .AddTo(this);

                viewModel.ItemCountVisible.Subscribe(
                    state => x.gameObject.SetActive(state));
            });
        }
    }
    public class ItemContainerView : ItemContainerView<ItemContainerViewModel<ItemContainer>, ItemContainer>
    {
    }
}
