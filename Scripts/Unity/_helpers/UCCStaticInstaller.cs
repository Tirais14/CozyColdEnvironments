#nullable enable
namespace CCEnvs.Unity
{
    /// <summary>
    /// !Destroys gameObject after installing
    /// </summary>
    public class UCCStaticInstaller : MonoCCStatic<UCCStaticInstaller>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            Install.Static();

            Destroy(gameObject);
        }
    }
}
