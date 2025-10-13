#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    public static class EditorSerializing
    {
        /// <summary>
        /// It doesn't work in the editor on purpose.
        /// </summary>
        public static void SetDefault<T>(ref T? fieldValue)
        {
#if !UNITY_EDITOR
            fieldValue = default;
#endif
        }
    }
}
