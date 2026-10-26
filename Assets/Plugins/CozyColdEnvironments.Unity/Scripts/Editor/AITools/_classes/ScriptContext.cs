#nullable enable
using CommunityToolkit.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace CCEnvs.UnityX
{
    public class ScriptContext
    {
        private readonly StringBuilder builder = new();

        private readonly HashSet<string> addedFiles = new();

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
                if (addedFiles.Contains(filePath))
                    return;

                addedFiles.Add(filePath);
                builder.AppendLine("//----File Start----");
                builder.Append("//----FilePath: ");
                builder.AppendLine(filePath);
                builder.AppendLine(content);
                builder.AppendLine("//----File End----");
            }
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
