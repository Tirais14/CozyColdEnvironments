#nullable enable
namespace CozyColdEnvironments.FileSystem.ScriptUtils
{
    public interface IContentProvider
    {
        IScriptContent[] Content { get; set; }
        bool HasContent { get; }
    }
}
