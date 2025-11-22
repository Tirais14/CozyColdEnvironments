#nullable enable
namespace CCEnvs.Unity.Components
{
    public class CCBehaviourComponentCommand : CCBehaviour
    {
        private int updateFrameCount;
        private int fixedUpdateFrameCount;
        private int lateUpdateFrameCount;

        protected override void Awake()
        {
            base.Awake();
            EnfOfFrameAction(() => Destroy(this));
            PreUpdateAction(OnPreUpdate);
        }

        protected virtual void OnPreUpdate()
        {

        }

        private void Update()
        {
            if (!didStart
                || 
                !didAwake 
                || 
                updateFrameCount > 0
                )
                return;

            OnUpdate();
            updateFrameCount++;
        }

        private void FixedUpdate()
        {
            if (!didStart 
                ||
                !didAwake
                ||
                fixedUpdateFrameCount > 0
                )
                return;

            OnFixedUpdate();
            fixedUpdateFrameCount++;
        }

        private void LateUpdate()
        {
            if (!didStart
                ||
                !didAwake
                || 
                lateUpdateFrameCount > 0
                )
                return;

            OnLateUpdate();
            lateUpdateFrameCount++;
        }

        protected virtual void OnDestroy()
        {

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
