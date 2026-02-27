using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Events;
using CCEnvs.Unity.Injections;
using UnityEngine;
using UnityEngine.UI;

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
