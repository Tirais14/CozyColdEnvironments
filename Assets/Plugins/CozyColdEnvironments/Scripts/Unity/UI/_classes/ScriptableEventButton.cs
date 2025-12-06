using CCEnvs.Unity.Components;
using CCEnvs.Unity.Events;
using UnityEngine.UI;
using UnityEngine;
using CCEnvs.Unity.Injections;
using CCEnvs.FuncLanguage;

#nullable enable
namespace CCEnvs.Unity.UI
{
    [RequireComponent(typeof(Button))]
    public class ScriptableEventButton : CCBehaviour
    {
        public Maybe<ScriptableEvent> buttonEvent;

        [GetBySelf]
        private Button m_Button = null!;

        public Button button => m_Button; 

        protected override void Awake()
        {
            base.Awake();
            button.onClick.AddListener(() => buttonEvent.IfSome(static x => x.Raise()));
        }
    }
}
