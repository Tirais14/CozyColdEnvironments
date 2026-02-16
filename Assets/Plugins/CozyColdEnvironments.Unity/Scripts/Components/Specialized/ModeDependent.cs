using UnityEngine;

namespace CCEnvs.Unity.Components.Specialized
{
    public sealed class ModeDependent : CCBehaviour
    {
        [Header("Modes Settings")]
        [Space(8f)]

        public bool destroyGameObjectOnExcluded = true;
        public bool excludeModes;

        [Header("Modes")]
        [Space(8f)]

        public bool debug = true;

        protected override void Awake()
        {
            base.Awake();

            if (!IsTargetMode())
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

        private bool IsTargetMode()
        {
            if (ModePredicate(CC.IsDebugMode) && debug)
                return false;

            return true;
        }

        private bool ModePredicate(bool modeState)
        {
            if (excludeModes)
                return modeState;

            return !modeState;
        }
    }
}
