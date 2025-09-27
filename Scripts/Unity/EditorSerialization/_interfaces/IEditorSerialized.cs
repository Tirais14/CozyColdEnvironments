#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    public interface IEditorSerialized
    {
        object Output { get; }
    }
    public interface IEditorSerialized<out T> : IEditorSerialized
    {
        new T Value { get; }

        object IEditorSerialized.Output => Value!;
    }
}
