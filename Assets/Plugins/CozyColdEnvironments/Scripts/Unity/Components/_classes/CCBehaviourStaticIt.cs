#nullable enable
namespace CCEnvs.Unity.Components
{
    public class CCBehaviourStaticIt<TThis> : CCBehaviourStatic<TThis>
        where TThis : CCBehaviourStatic
    {
        public static TThis It => self;
    }
}
