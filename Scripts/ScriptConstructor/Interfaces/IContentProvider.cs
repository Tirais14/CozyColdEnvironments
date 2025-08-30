#nullable enable
namespace CCEnvs.FileSystem.ScriptUtils
{
    public interface IContentProvider
    {
        IScriptContent[] Content { get; set; }
        bool HasContent { get; }
    }
}
