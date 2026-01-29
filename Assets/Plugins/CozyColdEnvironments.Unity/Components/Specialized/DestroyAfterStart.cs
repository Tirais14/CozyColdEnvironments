#nullable enable
namespace CCEnvs.Unity.Components.Specialized
{
    public class DestroyAfterStart : CCBehaviour
    {
        protected override void Start()
        {
            base.Start();

            Destroy(gameObject);
        }
    }
}
