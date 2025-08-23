#nullable enable
using UTIRLib.Diagnostics;

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
            if (FindAnyObjectByType(GetType()) != null)
            {
                TirLibDebug.PrintError($"{this.GetTypeName()} is static and cannot be created more than one time.");
                Destroy(this);
            }
        }
    }
    public abstract class MonoXStatic<TThis> : MonoXStatic
        where TThis : MonoXStatic
    {
        protected static TThis instance { get; private set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();
            instance = (this as TThis)!;
            if (instance == null)
                throw new TypeCastException(GetType(), typeof(TThis));
        }
    }
}