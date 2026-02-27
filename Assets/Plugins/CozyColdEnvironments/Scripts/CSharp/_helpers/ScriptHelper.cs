#nullable enable
using System.Linq;
using CCEnvs.Extensions;

namespace CCEnvs.CHSarp
{
    public static class ScriptHelper
    {
        public static string SetDefine(string scriptContent, string defineSymbol)
        {
            var lines = scriptContent.SplitByLines();

            if (lines.Any(line => line.StartsWith("#if") && line.Contains(defineSymbol)))
                return scriptContent;

            return $"#if {defineSymbol}\n{scriptContent}\n#endif";
        }
    }
}
