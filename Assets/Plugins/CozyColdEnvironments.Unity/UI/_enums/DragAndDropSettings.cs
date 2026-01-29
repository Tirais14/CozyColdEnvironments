using System;
using UnityEngine;

namespace CCEnvs.Unity.UI
{
    [Flags]
    public enum DragAndDropSettings
    {
        None,
        ResetPos,
        /// <summary>
        /// Creates a copy of this and do place it in empty space of moved drag item
        /// </summary>
        RefillEmptySpace = 2,
        SetAsLastSiblingWhenDragging = 4,
        /// <summary>
        /// Due dragging do placing this to special <see cref="Canvas"/> marked with <see cref="HighPriorityCanvasMarker"/>. If it doesn't exists prints error.
        /// </summary>
        InHighPriorityCanvas = 8,
        Default = None,
    }
}
