#nullable enable

namespace CozyColdEnvironments.FileSystem.ScriptUtils
{
    public class EmptyLineEntry : IScriptContent
    {
        int IScriptContent.TabulationsCount { get => 0; set { } }

        public override string ToString() => string.Empty;

        public static implicit operator string(EmptyLineEntry _) => string.Empty;
    }
}