using UnityEngine;

using Emulator = CCEnvs.Unity.Components.Specialized.PlatformDependentBehaviourEmulator;

#nullable enable
namespace CCEnvs.Unity.Components.Specialized
{
    public sealed class PlatformDependent : CCBehaviour
    {
        [Header("Platforms Settings")]
        [Space(8f)]

        public bool destroyGameObjectOnExcluded = true;
        public bool excludePlatforms;

        [Header("Platforms")]
        [Space(8f)]

        public bool webGL = true;
        public bool mobile;
        public bool console;

        protected override void Awake()
        {
            base.Awake();

            if (!IsTargetPlatform())
            {
                if (!destroyGameObjectOnExcluded)
                    gameObject.SetActive(false);
                else
                    Destroy(gameObject);
            }
        }

        protected override void Start()
        {
            base.Start();
            Destroy(this);
        }

        private bool IsTargetPlatform()
        {
            if (!Emulator.IsEnabled && Application.isEditor)
                return true;

            if (PlatformPredicate(UCC.Platform.IsWebGL) && webGL)
                return false;
            else if (PlatformPredicate(UCC.Platform.IsMobile) && mobile)
                return false;
            else if (PlatformPredicate(UCC.Platform.IsConsole) && console)
                return false;

            return true;
        }

        private bool PlatformPredicate(bool platformState)
        {
            if (excludePlatforms)
                return platformState;

            return !platformState;
        }
    }
}
