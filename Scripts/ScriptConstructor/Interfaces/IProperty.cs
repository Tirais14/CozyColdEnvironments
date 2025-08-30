#nullable enable
namespace CCEnvs.FileSystem.ScriptUtils
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
