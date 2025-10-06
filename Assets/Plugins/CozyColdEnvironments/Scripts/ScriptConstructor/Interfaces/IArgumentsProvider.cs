#nullable enable
namespace CCEnvs.Files.ScriptUtils
{
    public interface IArgumentsProvider
    {
        ArgumentEntry[] Arguments { get; set; }
        bool HasArguments { get; }

        void SetArguments(params object[] args);
    }
}
