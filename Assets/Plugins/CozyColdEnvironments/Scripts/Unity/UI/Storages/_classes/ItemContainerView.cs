using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Storages;
using CCEnvs.Unity.UI.MVVM;
using Cysharp.Threading.Tasks;
using Humanizer;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable enable
#pragma warning disable IDE0044
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

        protected override void Awake()
        {
            base.Awake();

            dragSettings = DragAndDropSettings.ResetPos
                           |
                           DragAndDropSettings.SetAsLastSiblingWhenDragging
                           |
                           DragAndDropSettings.InHighPriorityCanvas;

            showableSettings = IShowable.Settings.KeepRaycastTargetState;
        }

        protected override void InstallBingings()
        {
            base.InstallBingings();
            BindItemIcon();
            BindItemCount();
            BindActiveContainer();
        }

        public override void OnButtonClick()
        {
            viewModel.ActivateContainer();
        }

        public override bool DragAllowedPredicate()
        {
            if (model.IsEmpty)
            {
                this.PrintLog($"Dragging is not possible. {nameof(ItemContainer).Humanize()} is empty.");
                return false;
            }

            return true;
        }

        public override bool DropAllowedPredicate() => true;

        public override void Hide(IShowable.Settings settings)
        {
            base.Hide(settings);
            viewModel.DeactivateContainer();
        }

        protected override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            if (model.IsEmpty)
                viewModel.DeactivateContainer();
        }

        protected override void OnDrop(PointerEventData eventData)
        {
            if (!DropAllowedPredicate()
                || 
                eventData.pointerDrag == cGameObject.Value
                )
                return;

            eventData.pointerDrag.Maybe()
                .Map(go => go.FindFor().Model<IItemContainer>().Raw!)
                .Map(cnt => (source: cnt, rest: model.PutItem(cnt)))
                .Where(cnt => cnt.rest.IsSome)
                .IfSome(cnt => cnt.source.PutItem(cnt.rest.AccessUnsafe()));
        }

        private void BindItemIcon()
        {
            image.IfSome(img =>
            {
                viewModel.ItemIcon.SubscribeWithState(img,
                        static (sprite, img) => img.sprite = sprite)
                    .AddTo(this);
            });
        }

        private void BindItemCount()
        {
            counterMesh.IfSome(mesh =>
            {
                viewModel.ItemCount.SubscribeWithState(mesh,
                        static (text, mesh) => mesh.text = text)
                    .AddTo(this);
            });
        }

        private void BindActiveContainer()
        {
            viewModel.IsActiveContainer.SubscribeWithState(this, 
                    static (state, self) => state.Resolve()
                        .If(self.DoSelect)
                        .Else(self.DoDeselect))
                .AddTo(this);
        }
    }
    public class ItemContainerView : ItemContainerView<ItemContainerViewModel<ItemContainer>, ItemContainer>
    {
    }
}
