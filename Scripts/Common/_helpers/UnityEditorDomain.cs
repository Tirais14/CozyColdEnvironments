using CCEnvs.Diagnostics;

#nullable enable
#if UNITY_EDITOR
using UnityEditor;

namespace CCEnvs
{
    public static class UnityEditorDomain
    {
        public static int PlayModeEntranceCount { get; private set; }

        public static bool IsFirstPlayModeEntrance => PlayModeEntranceCount == 1;


        /// <summary>
        /// Dont use indirectly
        /// </summary>
        [InitializeOnLoadMethod]
        public static void OnDomainReload()
        {
            PlayModeEntranceCount = 0;

            CCDebug.PrintLog($"{nameof(PlayModeEntranceCount)} is reseted.",
                             new DebugContext(typeof(UnityEditorDomain)).Additive().Editor());
        }

        /// <summary>
        /// Dont use indirectly
        /// </summary>
        [InitializeOnEnterPlayMode]
        public static void OnPlayModeEnter()
        {
            PlayModeEntranceCount++;

            CCDebug.PrintLog($"{nameof(PlayModeEntranceCount)} = {PlayModeEntranceCount}",
                             new DebugContext(typeof(UnityEditorDomain)).Additive().Editor());
        }
    }
}
#else 

namespace CCEnvs
{
    public static class UnityEditorDomain
    {
        public static int PlayModeEntranceCount { get; } = 1;

        public static bool IsFirstPlayModeEntrance { get; } = true;
    }
}
#endif //UNITY_EDITOR
