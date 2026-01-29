using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public readonly struct ChangedDurabilityEvent
    {
        public float Previous { get; }
        public float Current { get; }
        public float Delta { get; }

        public ChangedDurabilityEvent(float previous, float current)
        {
            Previous = previous;
            Current = current;
            Delta = Mathf.Abs(current - previous);
        }
    }
}
