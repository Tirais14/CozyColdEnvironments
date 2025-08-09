#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public interface IProperty :
        ITypeMember,
        ITypeProvider,
        IUsingsProvider,
        IAttributesProvider
    {
        PropertyGetMethod? Getter { get; set; }
        PropertySetMethod? Setter { get; set; }
    }
}
