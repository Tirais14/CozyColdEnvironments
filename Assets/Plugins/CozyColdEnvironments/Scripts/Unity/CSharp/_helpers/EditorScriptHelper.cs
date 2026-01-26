using CCEnvs.FuncLanguage;
using SuperLinq;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.CSharp
{
    public static class EditorScriptHelper
    {
        public static async Task AddUnityEditorDefineSymbol(string folderPath, string[] byFolders, CancellationToken cancellationToken = default)
        {
            folderPath = Path.Combine(Application.dataPath + folderPath.Delete("Assets"));

            var scriptPaths = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

            foreach (var scriptPath in scriptPaths)
            {
                var scriptLines = await File.ReadAllLinesAsync(scriptPath, cancellationToken);

                if (!scriptPath.Split('/', '\\').Any(folderName => byFolders.Contains(folderName)))
                    continue;

                if (scriptLines.Any(line =>
                    {
                        return line.Contains("#if")
                               &&
                               line.Contains("UNITY_EDITOR")
                               &&
                               !line.Contains("UNITY_EDITOR_");
                    }))
                {
                    continue;
                }

                var scriptContent = string.Join(Environment.NewLine, scriptLines.Prepend("#if UNITY_EDITOR").Append("#endif"));

                await File.WriteAllTextAsync(scriptPath, scriptContent, cancellationToken);
            }
        }
    }
}
