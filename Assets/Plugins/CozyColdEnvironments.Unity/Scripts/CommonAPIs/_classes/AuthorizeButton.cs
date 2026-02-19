#if PLUGIN_YG_2 && Authorization_yg
using CCEnvs.Attributes;
using CCEnvs.Dependencies;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    [RequireComponent(typeof(Button))]
    public sealed class AuthorizeButton : CCBehaviour
    {
        [GetBySelf]
        private Button btn = null!;

        private IPlayerAPI playerAPI = null!;

        [field: SerializeField]
        public bool DestroyAfterAuthorize { get; set; }

        [field: SerializeField]
        public bool DisableAfterAuthorize { get; set; }

        [field: OptionalField]
        [field: SerializeField]
        public Transform? root { get; set; }

        protected override void Awake()
        {
            base.Awake();

            if (CCDependecyContainer.TryResolve<IPlayerAPI>().IsNull(out var playerAPI))
            {
                Destroy(GetRootOrSelfGameObject());
                return;
            }

            this.playerAPI = playerAPI;

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
            if (playerAPI.IsAuthorized)
                return;

            playerAPI.Authorize();
        }

        private GameObject GetRootOrSelfGameObject()
        {
            if (root == null)
                return gameObject;

            return root.gameObject;
        }

        private void OnAuthorized()
        {
            if (DestroyAfterAuthorize)
            {
                Destroy(GetRootOrSelfGameObject());
                return;
            }
            else if (DisableAfterAuthorize)
            {
                GetRootOrSelfGameObject().SetActive(false);
                return;
            }

            btn.interactable = false;
        }
    }
}

#endif //LUGIN_YG_2 && Authorization_yg
