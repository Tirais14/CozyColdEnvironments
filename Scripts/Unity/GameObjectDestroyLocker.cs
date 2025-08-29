#nullable enable

namespace CCEnvs.Unity
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