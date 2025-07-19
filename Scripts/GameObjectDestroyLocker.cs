#nullable enable

namespace UTIRLib
{
    public sealed class GameObjectDestroyLocker : MonoX
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