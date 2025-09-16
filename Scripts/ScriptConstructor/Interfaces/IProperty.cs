#nullable enable
namespace CCEnvs.Files.ScriptUtils
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
