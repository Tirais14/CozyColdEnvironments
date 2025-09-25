#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System.Linq;

namespace CCEnvs.Unity
{
    /// <summary>
    /// Do not add this or derived from this component! On scene must be only one <see cref="CCBehaviourStaticKernel"/>
    /// </summary>
    public abstract class CCBehaviourStatic : CCBehaviour
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            if (FindObjectsByType(GetType(),
                                  UnityEngine.FindObjectsInactive.Include,
                                  UnityEngine.FindObjectsSortMode.None)
                .Any(x => x != this))
            {
                CCDebug.PrintError($"{this.GetTypeName()} is static and cannot be created more than one time.", this);
                Destroy(this);
            }
        }
    }
    /// <summary>
    /// Do not add this or derived from this component! On scene must be only one <see cref="CCBehaviourStaticKernel"/>
    /// </summary>
    public abstract class CCBehaviourStatic<TThis> : CCBehaviourStatic
        where TThis : CCBehaviourStatic
    {
        private static TThis? instance;

        protected static TThis Instance {
            get
            {
                if (instance == null)
                    instance = CCBehaviourStaticKernel.GetInstance<TThis>();

                return instance;
            }
        }
    }
}