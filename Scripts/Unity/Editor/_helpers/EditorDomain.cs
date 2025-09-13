using CCEnvs.Diagnostics;
using UnityEditor;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public static class EditorDomain
    {
        public static int PlayModeEntranceCount { get; private set; }

        public static bool IsFirstPlayModeEntrance => PlayModeEntranceCount == 1;


        [InitializeOnLoadMethod]
        public static void OnDomainReload()
        {
            PlayModeEntranceCount = 0;

            CCDebug.PrintLog($"{nameof(PlayModeEntranceCount)} is reseted.",
                             new DebugContext(typeof(EditorDomain)).Additive().Editor());
        }

        [InitializeOnEnterPlayMode]
        public static void OnPlayModeEnter()
        {
            PlayModeEntranceCount++;

            CCDebug.PrintLog($"{nameof(PlayModeEntranceCount)} = {PlayModeEntranceCount}",
                             new DebugContext(typeof(EditorDomain)).Additive().Editor());
        }

        private static void InvokeOnEnterPlayModeResetMethods()
        {
            //(from assembly in AppDomain.CurrentDomain.GetAssemblies()
            // select assembly.GetTypes() into types
            // from type in types
            // select type.ForceGetMethods(BindingFlagsDefault.StaticAll) into methods
            // from method in methods
            // where method.IsDefined<ResetOnEnterPlayModeAttribute>(inherit: true)
            // where method.GetParameters().IsEmpty()
            // select method)
            // .ForEach(method => method.Invoke(obj: null, CC.EmptyArguments));
        }
    }
}
