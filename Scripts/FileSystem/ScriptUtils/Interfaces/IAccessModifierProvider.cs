#nullable enable
namespace CozyColdEnvironments.FileSystem.ScriptUtils
{
    public interface IAccessModifierProvider
    {
        Syntax.AccessModifier AccessModifier { get; set; }
    }
}
