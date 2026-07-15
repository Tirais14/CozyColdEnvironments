using CCEnvs.FuncLanguage;
using CCEnvs.UnityX.Components;
using UnityEngine;
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.UnityX.Events
{
    //[CreateAssetMenu(fileName = nameof(ScriptableEventListener), menuName = "Scriptable Objects/ScriptableEventListener")]
    public sealed class ScriptableEventListener : CCBehaviour
    {
        public Maybe<ScriptableEvent> scriptableEvent;
        public Maybe<UnityEvent> response;

        protected override void OnEnable()
        {
            if (this.scriptableEvent.TryGetValue(out ScriptableEvent? scriptableEvent))
                scriptableEvent.RegisterListener(this);
        }

        protected override void OnDisable()
        {
            if (this.scriptableEvent.TryGetValue(out ScriptableEvent? scriptableEvent))
                scriptableEvent.UnregisterListener(this);
        }

        public void OnEventRaised() => response.IfSome(x => x.Invoke());
    }
}
