#nullable enable
namespace CCEnvs.Unity
{
    public class CCBehaviourStaticQ<TThis> : CCBehaviourStatic<TThis>
        where TThis : CCBehaviourStatic
    {
        public static TThis Q => Self;
    }
}
