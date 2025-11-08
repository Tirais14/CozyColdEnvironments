#nullable enable
namespace CCEnvs.Unity.Components
{
    public class CCBehaviourTemporary : CCBehaviour
    {
        private int updateFrameCount;
        private int fixedUpdateFrameCount;
        private int lateUpdateFrameCount;

        protected override void Awake()
        {
            base.Awake();
            OnEndFrame += () => Destroy(this);
        }

        private void Update()
        {
            if (updateFrameCount > 0)
                return;

            OnUpdate();

            updateFrameCount++;
        }

        private void FixedUpdate()
        {
            if (fixedUpdateFrameCount > 0)
                return;

            OnFixedUpdate();

            fixedUpdateFrameCount++;
        }

        private void LateUpdate()
        {
            if (lateUpdateFrameCount > 0)
                return;

            OnLateUpdate();

            lateUpdateFrameCount++;
        }

        /// <summary>
        /// Invokes only once
        /// </summary>
        protected virtual void OnUpdate()
        {
        }

        /// <summary>
        /// Invokes only once
        /// </summary>
        protected virtual void OnFixedUpdate()
        {
        }

        /// <summary>
        /// Invokes only once
        /// </summary>
        protected virtual void OnLateUpdate()
        {
        }
    }
}
