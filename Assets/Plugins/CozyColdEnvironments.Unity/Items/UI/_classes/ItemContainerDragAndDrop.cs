using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.UI;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.Items.UI
{
    public class ItemContainerDragAndDrop : DragAndDropTarget
    {
        [GetBySelf]
        private IItemContainer itemContainer = null!;

        protected override void Awake()
        {
            base.Awake();
            dragSettings = DragAndDropSettings.ResetPos
                           |
                           DragAndDropSettings.SetAsLastSiblingWhenDragging
                           |
                           DragAndDropSettings.InHighPriorityCanvas;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool DragPredicate()
        {

            return base.DragPredicate() && itemContainer.ContainsItem();
        }

        protected override void OnDrop(PointerEventData eventData)
        {
            base.OnDrop(eventData);

            if (!DropPredicate()
                ||
                eventData.pointerDrag == cGameObject.Value
                )
                return;

            eventData.pointerDrag.Maybe()
                .Map(go => go.QueryTo().Model<IItemContainer>().Raw!)
                .Map(cnt => (source: cnt, rest: itemContainer.PutItemFrom(cnt)))
                .Where(cnt => cnt.rest.IsSome)
                .IfSome(cnt => cnt.source.PutItemFrom(cnt.rest.GetValueUnsafe()));
        }
    }
}
