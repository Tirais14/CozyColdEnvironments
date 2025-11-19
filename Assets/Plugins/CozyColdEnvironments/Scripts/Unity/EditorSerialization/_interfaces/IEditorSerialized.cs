#nullable enable
namespace CCEnvs.Unity.Serialization
{
    public interface IEditorSerialized
    {
        object Value { get; }
    }
    public interface IEditorSerialized<out T> : IEditorSerialized
    {
        new T Value { get; }

        object IEditorSerialized.Value => Value!;
    }
}
