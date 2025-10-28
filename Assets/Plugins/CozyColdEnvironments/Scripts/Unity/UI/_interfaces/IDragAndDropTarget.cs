#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IDragAndDropTarget
    {
        /// <summary>
        /// Count of <see cref="DragHandler"/> and <see cref="DropHandler"/> binding count. While is bigger than 1, <see cref="DeactivateDragAndDropAbility"/> only decreases count
        /// </summary>
        int BindingCount { get; }

        /// <summary>
        /// Must be increase <see cref="BindingCount"/> by one
        /// </summary>
        void ActivateDragAndDropAbility();

        /// <summary>
        /// Must be decrease <see cref="BindingCount"/> by one
        /// </summary>
        void DeactivateDragAndDropAbility();
    }
}
