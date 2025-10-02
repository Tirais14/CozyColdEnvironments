using CCEnvs.Reflection;

namespace CCEnvs.ReflectedTypes
{
    public sealed class EditorDomainReflected : Reflected
    {
#if UNITY_EDITOR
        public int PlayModeEntranceCount {
            get => Property(nameof(PlayModeEntranceCount)).GetValue().As<int>();
        }

        public bool IsFirstPlayModeEntrance {
            get => Property(nameof(IsFirstPlayModeEntrance)).GetValue().As<bool>();
        }
#else
        public int PlayModeEntranceCount => 1;

        public bool IsFirstPlayModeEntrance => true;
#endif

#if UNITY_EDITOR
        public EditorDomainReflected()
            :
            base(TypeSearch.FindTypeInAppDomain(new TypeSearchArguments
            {
                AssemblyName = Syntax.Chain(nameof(CCEnvs), "Unity", "Editor"),
                TypeName = "EditorDomain",
            }, throwOnError: true),
            instance: null)
        {
        }
#endif
    }
}
