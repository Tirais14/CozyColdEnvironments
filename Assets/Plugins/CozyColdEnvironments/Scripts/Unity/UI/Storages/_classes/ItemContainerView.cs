using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Storages;
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

            showableDisableGraphicsSettings = DisableGraphicsSettings.KeepRaycastTargetState;
        }

        protected override void Start()
        {
            base.Start();

            BindItemIcon();
            BindItemCount();
            BindActiveContainer();
        }

        protected override bool DragPredicate(out Maybe<string> msg)
        {
            if (model.IsEmpty)
            {
                msg = $"Dragging is not possible. {nameof(ItemContainer)} is empty.";
                return false;
            }

            msg = null;
            return true;
        }

        protected override void OnDrop(PointerEventData eventData)
        {
            if (!DropPredicate()
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
                viewModel.ItemIcon.SubscribeWithState(img, static (sprite, img) => img.sprite = sprite)
                                  .AddTo(this);

                viewModel.ItemIconVisible.SubscribeWithState(this, static (state, self) => state.Resolve().Match(self.Show, self.Hide))
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

                viewModel.ItemCountVisible.SubscribeWithState(mesh,
                    static (state, mesh) => mesh.gameObject.SetActive(state));
            });
        }

        private void BindActiveContainer()
        {
            viewModel.IsActiveContainer.SubscribeWithState(this, (state, self) =>
            {
                if (state)
                    self.DoSelect();
                else
                    self.DoDeselect();
            });
        }
    }
    public class ItemContainerView : ItemContainerView<ItemContainerViewModel<ItemContainer>, ItemContainer>
    {
        protected override void SetupViewModel()
        {
        }
    }
}
