using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.Proeprties
{
    public interface ITrigger<T>
    {
        T? PeekValue();

        T? GetValue();

        bool TryGetValue([NotNullWhen(true)] out T? value);

        void SetValue(T? value);
    }
    public interface ITrigger : ITrigger<bool> { }
}
