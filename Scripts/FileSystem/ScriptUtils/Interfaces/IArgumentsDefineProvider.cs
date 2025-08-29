#nullable enable
namespace CozyColdEnvironments.FileSystem.ScriptUtils
{
    public interface IArgumentsDefineProvider
    {
        ArgumentDefineEntry[] DefineArguments { get; set; }
        bool HasDefineArguments { get; }
    }
}
