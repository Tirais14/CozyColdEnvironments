#nullable enable
using System.Linq;

namespace UTIRLib
{
    /// <summary>
    /// Do not add this or derived from this component! On scene must be only one <see cref="MonoXStaticCore"/>
    /// </summary>
    public abstract class MonoXStatic : MonoX
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            if (FindObjectsByType(GetType(),
                                  UnityEngine.FindObjectsInactive.Include,
                                  UnityEngine.FindObjectsSortMode.None)
                .Any(x => x != this))
            {
                TirLibDebug.PrintError($"{this.GetTypeName()} is static and cannot be created more than one time.", this);
                Destroy(this);
            }
        }
    }
    /// <summary>
    /// Do not add this or derived from this component! On scene must be only one <see cref="MonoXStaticCore"/>
    /// </summary>
    public abstract class MonoXStatic<TThis> : MonoXStatic
        where TThis : MonoXStatic
    {
        private static TThis? instance;

        protected static TThis Instance {
            get
            {
                if (instance == null)
                    instance = MonoXStaticCore.GetInstance<TThis>();

                return instance;
            }
        }
    }
}