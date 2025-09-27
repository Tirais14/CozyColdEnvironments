#nullable enable

namespace CCEnvs.Unity.Components.Specialized
{
    public sealed class DontDestroyOnLoad : CCBehaviour
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);

            Destroy(this);
        }
    }
}