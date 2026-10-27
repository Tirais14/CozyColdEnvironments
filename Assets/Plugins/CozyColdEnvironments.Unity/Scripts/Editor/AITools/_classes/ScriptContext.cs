#nullable enable
using CommunityToolkit.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace CCEnvs.UnityX
{
    public class ScriptContext
    {
        private readonly Dictionary<string, string> addedFiles = new();

        public int ScriptCount { get; private set; }

        public object Gate { get; } = new();

        public void AddScriptContent(string filePath, string content)
        {
            Guard.IsNotNullOrWhiteSpace(filePath, nameof(filePath));
            Guard.IsNotNull(content, nameof(content));

            if (content.IsNullOrWhiteSpace())
                return;

            lock (Gate)
            {
                if (addedFiles.TryGetValue(filePath, out var otherContent))
                {
                    if (content.Length > otherContent.Length)
                        addedFiles[filePath] = otherContent;
                    else
                        return;
                }

                addedFiles.Add(filePath, content);
            }
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(string? additional)
        {
            var sb = new StringBuilder();
            int i = 0;
            foreach (var file in addedFiles)
            {
                if (i++ == 0)
                {
                    sb.AppendLine("//----Query Start----");
                    sb.Append("//");
                    sb.AppendLine("The result of the edits should be a single file that contains metadata about where each script file begins. Also, only those script files that have been modified in any way should be added to the resulting file, and in full — both modified and unmodified lines. Each script file should begin with //----File Start----, followed on the next line by //----FilePath: \"Absolute Path\"----. For scripts that are no longer needed for any purpose, after FilePath a marker //----Deleted---- should be placed; the contents of the deleted file itself need not be included. At the end of the script file, a marker //----File End---- should be placed.");

                    if (additional.IsNotNullOrWhiteSpace())
                        sb.AppendLine(additional);

                    sb.AppendLine("//----Query End----");
                }

                sb.AppendLine("//----File Start----");
                sb.Append("//----FilePath: ");
                sb.AppendLine(file.Key);
                sb.AppendLine(file.Value);
                sb.AppendLine("//----File End----");
            }

            return sb.ToString();
        }
    }
}
