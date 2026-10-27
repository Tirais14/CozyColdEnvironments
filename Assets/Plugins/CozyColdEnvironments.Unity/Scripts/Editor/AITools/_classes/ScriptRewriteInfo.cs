using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.UnityX.Editr.AITools
{
    public class ScriptRewriteInfo
    {
        public string Content { get; }

        public string FilePath { get; }

        public bool ToDelete { get; set; }

        public ScriptRewriteInfo(
            string content,
            string filePath
            )
        {
            Guard.IsNotNull(content, nameof(content));
            Guard.IsNotNull(filePath, nameof(filePath));

            Content = content;
            FilePath = filePath;
        }
    }
}
