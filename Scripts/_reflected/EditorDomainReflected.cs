using CCEnvs.Reflection;

namespace CCEnvs.ReflectedTypes
{
    public sealed class EditorDomainReflected : Reflected
    {
        public int PlayModeEntranceCount {
            get => Property(nameof(PlayModeEntranceCount)).GetValue().As<int>();
        }

        public bool IsFirstPlayModeEntrance {
            get => Property(nameof(IsFirstPlayModeEntrance)).GetValue().As<bool>();
        }

        public EditorDomainReflected()
            :
            base(TypeFinder.FindTypeInAppDomain(new TypeFinderParameters
            {
                AssemblyName = new OperatorChain(nameof(CCEnvs), "Unity", "Editor"),
                TypeName = "EditorDomain",
            }, throwOnError: false),
            target: null)
        {
        }
    }
}
