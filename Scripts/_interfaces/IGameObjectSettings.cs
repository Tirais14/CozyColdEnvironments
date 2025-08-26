#nullable enable
using UnityEngine;

namespace UTIRLib
{
    public interface IGameObjectSettings
    {
        Vector3 Scale { get; }

        void ApplyTo(GameObject gameObject);
    }
    public interface IGameObjectSettings<T> : IGameObjectSettings
        where T : Component
    {
        void ApplyTo(T component);
    }
}
