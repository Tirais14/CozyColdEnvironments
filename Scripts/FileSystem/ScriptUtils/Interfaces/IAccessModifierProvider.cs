#nullable enable
namespace CCEnvs.FileSystem.ScriptUtils
{
    public interface IAccessModifierProvider
    {
        Syntax.AccessModifier AccessModifier { get; set; }
    }
}
