#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public interface IArgumentsDefineProvider
    {
        ArgumentDefineEntry[] DefineArguments { get; set; }
        bool HasDefineArguments { get; }
    }
}
