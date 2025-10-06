#nullable enable
namespace CCEnvs.Unity.Components
{
    public class CCBehaviourStaticQ<TThis> : CCBehaviourStatic<TThis>
        where TThis : CCBehaviourStatic
    {
        public static TThis Q => self;
    }
}
