using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using UnityEngine;
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.Unity.Events
{
    [CreateAssetMenu(fileName = nameof(ScriptableEventListener), menuName = "Scriptable Objects/ScriptableEventListener")]
    public sealed class ScriptableEventListener : CCBehaviour
    {
        public Maybe<ScriptableEvent> scriptableEvent;
        public Maybe<UnityEvent> response;

        private void OnEnable()
        {
            if (this.scriptableEvent.TryGetValue(out ScriptableEvent? scriptableEvent))
                scriptableEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            if (this.scriptableEvent.TryGetValue(out ScriptableEvent? scriptableEvent))
                scriptableEvent.UnregisterListener(this);
        }

        public void OnEventRaised() => response.IfSome(x => x.Invoke());
    }
}
