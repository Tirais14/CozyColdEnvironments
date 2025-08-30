#nullable enable
namespace CCEnvs.FileSystem.ScriptUtils
{
    public interface IArgumentsDefineProvider
    {
        ArgumentDefineEntry[] DefineArguments { get; set; }
        bool HasDefineArguments { get; }
    }
}
