#nullable enable
namespace CCEnvs.Files.ScriptUtils
{
    public interface IAccessModifierProvider
    {
        Syntax.AccessModifier AccessModifier { get; set; }
    }
}
