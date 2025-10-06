#nullable enable
namespace CCEnvs.Files.ScriptUtils
{
    public interface IContentProvider
    {
        IScriptContent[] Content { get; set; }
        bool HasContent { get; }
    }
}
