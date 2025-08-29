#nullable enable
namespace CozyColdEnvironments.FileSystem.ScriptUtils
{
    public interface IArgumentsProvider
    {
        ArgumentEntry[] Arguments { get; set; }
        bool HasArguments { get; }

        void SetArguments(params object[] args);
    }
}
