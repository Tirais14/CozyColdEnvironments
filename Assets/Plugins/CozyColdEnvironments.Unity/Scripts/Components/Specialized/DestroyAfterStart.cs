#nullable enable
namespace CCEnvs.UnityX.Components.Specialized
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
