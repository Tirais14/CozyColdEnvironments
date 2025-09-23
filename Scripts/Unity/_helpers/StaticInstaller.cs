#nullable enable
namespace CCEnvs.Unity
{
    /// <summary>
    /// !Destroys gameObject after installing
    /// </summary>
    public class StaticInstaller : MonoCCStatic<StaticInstaller>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            Install.Static();

            Destroy(gameObject);
        }
    }
}
