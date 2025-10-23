#nullable enable
namespace CCEnvs.Language
{
    public interface IConditional
    {
        bool IsSome { get; }
        bool IsNone { get; }
    }
    public interface IConditional<out T> : IConditional
    {
        T? Value();
    }
}
