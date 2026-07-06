#nullable enable
using CCEnvs.Extensions;
using CommunityToolkit.Diagnostics;
using System;
using System.Linq;
using System.Text.RegularExpressions;

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

        public static string SetNamespace(string scriptContent, string ns)
        {
            Guard.IsNotNull(scriptContent);
            Guard.IsNotNull(ns);

            if (!Regex.IsMatch(ns, @"^[\w.]+$"))
                throw new ArgumentException($"Невалидное имя namespace: {ns}");

            string pattern = @"^(\s*)namespace\s+[\w.]+";
            string replacement = $"$1namespace {ns}";

            string result = Regex.Replace(
                scriptContent,
                pattern,
                replacement,
                RegexOptions.Multiline
                );

            if (result == scriptContent)
            {
                result = InsertNewNamespace(scriptContent, ns);
            }

            return result;
        }

        private static string InsertNewNamespace(string scriptContent, string newNamespace)
        {
            var usingMatches = Regex.Matches(scriptContent, @"^using\s+.*?;", RegexOptions.Multiline);

            int insertIndex = 0;

            if (usingMatches.Count > 0)
            {
                var lastUsing = usingMatches[^1];
                insertIndex = lastUsing.Index + lastUsing.Length;

                while (insertIndex < scriptContent.Length
                       &&
                      (scriptContent[insertIndex] == '\r' || scriptContent[insertIndex] == '\n'))
                {
                    insertIndex++;
                }
            }

            string before = scriptContent[..insertIndex];
            string after = scriptContent[insertIndex..];

            return $"{before}\nnamespace {newNamespace}\n{{\n{after}\n}}\n";
        }
    }
}
