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

        [Tooltip("Valid if all platforms completes predicate")]
        public bool andInsteadOr;

        [Header("Platforms")]
        [Space(8f)]

        public bool webGL;
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

            if (andInsteadOr)
            {
                return (!webGL || PlatformPredicate(UCC.Platform.IsWebGL))
                       &&
                       (!mobile || PlatformPredicate(UCC.Platform.IsMobile))
                       &&
                       (!console || PlatformPredicate(UCC.Platform.IsConsole));
            }

            if (webGL && PlatformPredicate(UCC.Platform.IsWebGL))
                return true;
            else if (mobile && PlatformPredicate(UCC.Platform.IsMobile))
                return true;
            else if (console && PlatformPredicate(UCC.Platform.IsConsole))
                return true;

            return false;
        }

        private bool PlatformPredicate(bool platformState)
        {
            if (excludePlatforms)
                return false;

            return platformState;
        }
    }
}
