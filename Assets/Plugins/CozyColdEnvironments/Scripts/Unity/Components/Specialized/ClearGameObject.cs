#nullable enable
namespace CCEnvs.Unity.Components.Specialized
{
    public class ClearGameObject : CCBehaviour
    {
        protected override void Start()
        {
            base.Start();
            gameObject.RemoveComponents();
            Destroy(this);
        }
    }
}
