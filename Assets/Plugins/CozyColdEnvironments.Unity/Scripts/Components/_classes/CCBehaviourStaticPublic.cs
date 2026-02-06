#nullable enable
namespace CCEnvs.Unity.Components
{
    public class CCBehaviourStaticPublic<TThis> : CCBehaviourStatic<TThis>
        where TThis : CCBehaviourStatic
    {
        public static TThis Self => self;
    }
}
