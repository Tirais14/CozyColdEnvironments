using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    public interface IEditorSerialized
    {
        object Output { get; }
    }
    public interface IEditorSerialized<out T> : IEditorSerialized
    {
        new T Output { get; }

        object IEditorSerialized.Output => Output!;
    }
}
