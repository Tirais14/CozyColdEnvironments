#nullable enable

namespace CCEnvs.Unity.Components.Specialized
{
    public sealed class DontDestroyOnLoad : CCBehaviour
    {
        protected override void Awake()
        {
            base.Awake();

            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);

            Destroy(this);
        }
    }
}