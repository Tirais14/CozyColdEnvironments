#nullable enable
using CCEnvs.Unity.Components;

namespace CCEnvs.Unity
{
    /// <summary>
    /// !Destroys gameObject after installing
    /// </summary>
    public class StaticInstaller : CCBehaviourStatic<StaticInstaller>
    {
        protected override void Awake()
        {
            base.Awake();
            Install.Static();

            Destroy(gameObject);
        }
    }
}
