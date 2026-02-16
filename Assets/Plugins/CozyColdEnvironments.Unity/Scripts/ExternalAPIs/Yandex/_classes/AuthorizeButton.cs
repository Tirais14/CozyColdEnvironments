#if PLUGIN_YG_2 && Authorization_yg
using CCEnvs.Dependencies;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    [RequireComponent(typeof(Button))]
    public class AuthorizeButton : CCBehaviour
    {
        [GetBySelf()]
        private Button btn = null!;

        private IPlayerAPI playerAPI = null!;

        protected override void Awake()
        {
            base.Awake();

            playerAPI = BuiltInDependecyContainer.Resolve<IPlayerAPI>();

            if (playerAPI.IsAuthorized)
                OnAuthorized();
        }

        protected override void Start()
        {
            base.Start();

            btn.onClick.AddListener(Authorize);
        }

        private void Authorize()
        {
            playerAPI.Authorize();
        }

        private void OnAuthorized()
        {
            btn.interactable = false;
        }
    }
}

#endif //LUGIN_YG_2 && Authorization_yg
