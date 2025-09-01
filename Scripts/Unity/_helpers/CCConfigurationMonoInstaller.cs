#nullable enable
namespace CCEnvs.Unity
{
    /// <summary>
    /// !Destroys gameObject after installing
    /// </summary>
    public class CCConfigurationMonoInstaller : MonoCCStatic<CCConfigurationMonoInstaller>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            CCConfigurationInstaller.Install();

            Destroy(gameObject);
        }
    }
}
