#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System.Linq;
using UnityEngine;

#pragma warning disable IDE1006
namespace CCEnvs.Unity.Components
{
    /// <summary>
    /// Same as the singleton. Auto initalizes on scene with the first member access or instant by the <see cref="Attributes.InstantCreationAttribute"/>
    /// </summary>
    public abstract class CCBehaviourStatic : CCBehaviour
    {
        private CCBehaviourStatic? _self;

        protected CCBehaviourStatic self {
            get
            {
                if (_self == null)
                    _self = CCBehaviourStaticKernel.GetInstance(GetType());

                return _self;
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
                this.PrintError($"{this.GetTypeName()} is static and cannot be created more than one time.");
                this.PrintLog(StackTraceUtility.ExtractStackTrace());
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
        private static TThis? _self;

        protected static new TThis self {
            get
            {
                if (_self == null)
                    _self = CCBehaviourStaticKernel.GetInstance<TThis>();

                return _self;
            }
        }
    }
}