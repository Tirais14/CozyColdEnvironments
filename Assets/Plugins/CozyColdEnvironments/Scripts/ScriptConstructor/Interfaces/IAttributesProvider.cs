#nullable enable
namespace CCEnvs.Files.ScriptUtils
{
    public interface IAttributesProvider
    {
        AttributeEntry[] Attributes { get; set; }
        bool HasAttributes { get; }
    }
}
