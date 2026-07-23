using CCEnvs.FuncLanguage;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Events;
using CCEnvs.UnityX.ComponentInjections;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.UnityX.UI
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
