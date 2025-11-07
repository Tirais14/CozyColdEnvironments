using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Storages;
using CCEnvs.Unity.UI.MVVM;
using Cysharp.Threading.Tasks;
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

        protected override bool showOnStart => true;

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

        protected override bool DragAllowedPredicate(out Maybe<string> msg)
        {
            if (model.IsEmpty)
            {
                msg = $"Dragging is not possible. {nameof(ItemContainer)} is empty.";
                return false;
            }

            msg = null;
            return true;
        }

        protected override bool DropAllowedPredicate(out Maybe<string> msg)
        {
            msg = null;

            return true;
        }

        protected override void OnDrop(PointerEventData eventData)
        {
            if (!DropAllowedPredicate(out _)
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
            Img.IfSome(img =>
            {
                viewModel.ItemIcon.SubscribeWithState(img,
                    static (sprite, img) => img.sprite = sprite)
                    .AddTo(this);

                viewModel.ItemIconVisible.SubscribeWithState(this,
                    static (state, self) => state.Resolve().If(self.Show).Else(self.Hide))
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

                viewModel.ItemCountVisible.SubscribeWithState2(this, mesh,
                    static (state, self, mesh) => self.IsVisible.Resolve().If(() => mesh.gameObject.SetActive(state)))
                    .AddTo(this);
            });
        }

        private void BindActiveContainer()
        {
            viewModel.IsActiveContainer.SubscribeWithState(this, (state, self) => state.Resolve()
                    .If(self.DoSelect)
                    .Else(self.DoDeselect))
                .AddTo(this);
        }
    }
    public class ItemContainerView : ItemContainerView<ItemContainerViewModel<ItemContainer>, ItemContainer>
    {
    }
}
