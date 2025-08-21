#nullable enable
using UnityEngine;

namespace UTIRLib.InputSystem
{
    public interface IPointerInput : IInputAction<Vector2>
    {
        Vector2 GetPointerWorldPosition();
    }
}
