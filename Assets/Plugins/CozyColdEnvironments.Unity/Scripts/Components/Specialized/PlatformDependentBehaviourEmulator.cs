using UnityEngine;
#nullable enable
namespace CCEnvs.Unity.Components.Specialized
{
    public sealed class PlatformDependentBehaviourEmulator : CCBehaviourStaticPublic<PlatformDependentBehaviourEmulator>
    {
        public static bool IsWebGL {
            get
            {
#if UNITY_EDITOR
                return IsEnabled && self.webGL;
#else
                return false;
#endif
            }
        }

        public static bool IsMobile {
            get
            {
#if UNITY_EDITOR
                return IsEnabled && self.mobile;
#else
                return false;
#endif
            }
        }

        public static bool IsConsole {
            get
            {
#if UNITY_EDITOR
                return IsEnabled && self.console;
#else
                return false;
#endif
            }
        }

        public static bool IsEnabled {
            get
            {
#if UNITY_EDITOR
                return self.isEnabled;
#else
                return false;
#endif
            }
        }

        [SerializeField]
        private bool isEnabled;

        [Header("Platforms")]
        [Space(8f)]

        [SerializeField]
        private bool webGL;

        [SerializeField]
        private bool mobile;

        [SerializeField]
        private bool console;
    }
}
