using UnityEngine;

#nullable enable
namespace UTIRLib.UI
{
    public interface IMovable
    {
        Vector2 Position { get; set; }

        void ResetPosition();
    }
}
