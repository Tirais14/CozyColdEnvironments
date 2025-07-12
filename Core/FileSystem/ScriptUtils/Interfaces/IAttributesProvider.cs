#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public interface IAttributesProvider
    {
        AttributeEntry[] Attributes { get; set; }
        bool HasAttributes { get; }
    }
}
