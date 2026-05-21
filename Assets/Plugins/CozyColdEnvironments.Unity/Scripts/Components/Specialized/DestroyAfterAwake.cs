#nullable enable
namespace CCEnvs.UnityX.Components.Specialized
{
    public sealed class DestroyAfterAwake : CCBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            Destroy(gameObject);
        }
    }
}
