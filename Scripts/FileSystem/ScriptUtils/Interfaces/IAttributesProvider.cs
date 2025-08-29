#nullable enable
namespace CozyColdEnvironments.FileSystem.ScriptUtils
{
    public interface IAttributesProvider
    {
        AttributeEntry[] Attributes { get; set; }
        bool HasAttributes { get; }
    }
}
