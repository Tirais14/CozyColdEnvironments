using UnityEngine;
using UnityEngine.Serialization;
using Emulator = CCEnvs.Unity.Components.Specialized.PlatformDependentBehaviourEmulator;

#nullable enable
namespace CCEnvs.Unity.Components.Specialized
{
    public sealed class PlatformDependent : CCBehaviour
    {
        [Header("Platforms Settings")]
        [Space(8f)]

        public bool destroyGameObjectOnExcluded = true;

        [FormerlySerializedAs("excludePlatforms")]
        public bool isBlackList;

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

            bool isWebGL = UCC.Platform.IsWebGL;
            bool isMobile = UCC.Platform.IsMobile;
            bool isConsole = UCC.Platform.IsConsole;

            bool hasAnyPlatformSelected = webGL || mobile || console;

            if (isBlackList)
            {
                if (!hasAnyPlatformSelected)
                    return true;

                if (andInsteadOr)
                {
                    bool allExcluded = true;

                    if (webGL && !isWebGL)
                        allExcluded = false;
                    if (mobile && !isMobile)
                        allExcluded = false;
                    if (console && !isConsole)
                        allExcluded = false;

                    return !allExcluded;
                }
                else
                {
                    if (webGL && isWebGL)
                        return false;
                    if (mobile && isMobile)
                        return false;
                    if (console && isConsole)
                        return false;

                    return true;
                }
            }
            else
            {
                if (!hasAnyPlatformSelected)
                    return false;

                if (andInsteadOr)
                {
                    bool allIncluded = true;

                    if (webGL && !isWebGL)
                        allIncluded = false;
                    if (mobile && !isMobile)
                        allIncluded = false;
                    if (console && !isConsole)
                        allIncluded = false;

                    return allIncluded;
                }
                else
                {
                    if (webGL && isWebGL)
                        return true;
                    if (mobile && isMobile)
                        return true;
                    if (console && isConsole)
                        return true;

                    return false;
                }
            }
        }

        private bool PlatformPredicate(bool platformState)
        {
            if (isBlackList)
                return !platformState;

            return platformState;
        }
    }
}
