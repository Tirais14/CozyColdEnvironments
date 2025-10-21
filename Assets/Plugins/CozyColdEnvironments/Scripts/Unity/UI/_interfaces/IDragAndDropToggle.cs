#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IDragAndDropToggle
    {
        /// <summary>
        /// Count of <see cref="DragHandler"/> and <see cref="DropHandler"/> binding count. While is bigger than 1, <see cref="DeactivateDragAndDropAbility"/> only decreases count
        /// </summary>
        int DragAndDropHandlerBindingCount { get; }

        /// <summary>
        /// Must be increase <see cref="DragAndDropHandlerBindingCount"/> by one
        /// </summary>
        void ActivateDragAndDropAbility();

        /// <summary>
        /// Must be decrease <see cref="DragAndDropHandlerBindingCount"/> by one
        /// </summary>
        void DeactivateDragAndDropAbility();
    }
}
