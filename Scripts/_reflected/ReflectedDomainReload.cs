using CCEnvs.Reflection;

namespace CCEnvs.ReflectedTypes
{
    public sealed class ReflectedDomainReload : Reflected
    {
        public bool IsReloaded {
            get
            {
                if (TargetType is null)
                    return true;

                return Property("IsReloaded").GetValue().As<bool>();
            }
        }

        public ReflectedDomainReload()
            :
            base(TypeFinder.FindTypeInAppDomain(new TypeFinderParameters
            {
                Namespace = CC.Create.Array(nameof(CCEnvs), "Unity", "Editor").JoinStrings('.'),
                TypeName = "DomainReload",
            }, throwOnError: false))
        {
        }
    }
}
