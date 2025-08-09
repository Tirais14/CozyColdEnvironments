#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public interface IContentProvider
    {
        IScriptContent[] Content { get; set; }
        bool HasContent { get; }
    }
}
