#nullable enable
namespace CCEnvs.FileSystem.ScriptUtils
{
    public interface IAttributesProvider
    {
        AttributeEntry[] Attributes { get; set; }
        bool HasAttributes { get; }
    }
}
