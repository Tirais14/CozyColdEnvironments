#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System.Linq;

namespace CCEnvs.Unity
{
    /// <summary>
    /// Same as the singleton. Auto initalizes on scene with the first member access or instant by the <see cref="Attributes.InstantCreationAttribute"/>
    /// </summary>
    public abstract class CCBehaviourStatic : CCBehaviour
    {
        private CCBehaviourStatic? self;

        protected CCBehaviourStatic Self {
            get
            {
                if (self == null)
                    self = CCBehaviourStaticKernel.GetInstance(GetType());

                return self;
            }
        }

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
                return;
            }

            this.PrintLog("Awaked");
        }
    }
    /// <summary>
    /// Do not add this or derived from this component! On scene must be only one <see cref="CCBehaviourStaticKernel"/>
    /// </summary>
    public abstract class CCBehaviourStatic<TThis> : CCBehaviourStatic
        where TThis : CCBehaviourStatic
    {
        private static TThis? self;

        new protected static TThis Self {
            get
            {
                if (self == null)
                    self = CCBehaviourStaticKernel.GetInstance<TThis>();

                return self;
            }
        }
    }
}