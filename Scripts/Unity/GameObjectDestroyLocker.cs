#nullable enable

namespace CozyColdEnvironments
{
    public sealed class GameObjectDestroyLocker : MonoCC
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