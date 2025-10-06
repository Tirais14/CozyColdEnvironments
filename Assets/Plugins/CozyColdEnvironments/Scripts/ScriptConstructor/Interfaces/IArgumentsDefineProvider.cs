#nullable enable
namespace CCEnvs.Files.ScriptUtils
{
    public interface IArgumentsDefineProvider
    {
        ArgumentDefineEntry[] DefineArguments { get; set; }
        bool HasDefineArguments { get; }
    }
}
