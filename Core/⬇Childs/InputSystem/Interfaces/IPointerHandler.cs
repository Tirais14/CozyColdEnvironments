using UnityEngine;

namespace UTIRLib.InputSystem
{
    public interface IPointerHandler
    {
        public Vector2 PointerPosition { get; }
        public Vector2 WorldPointerPosition { get; }
    }
}