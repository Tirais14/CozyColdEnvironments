using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Events
{
    [CreateAssetMenu(fileName = "ScriptableEvent", menuName = "Scriptable Objects/ScriptableEvent")]
    public sealed class ScriptableEvent : ScriptableObject
    {
        private readonly List<ScriptableEventListener> listeners = new();

        public void Raise() => listeners.ForEach(l => l.OnEventRaised());
        public void RegisterListener(ScriptableEventListener listener) => listeners.Add(listener);
        public void UnregisterListener(ScriptableEventListener listener) => listeners.Remove(listener);
    }
}
