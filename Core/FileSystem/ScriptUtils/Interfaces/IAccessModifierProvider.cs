#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public interface IAccessModifierProvider
    {
        Syntax.AccessModifier AccessModifier { get; set; }
    }
}
