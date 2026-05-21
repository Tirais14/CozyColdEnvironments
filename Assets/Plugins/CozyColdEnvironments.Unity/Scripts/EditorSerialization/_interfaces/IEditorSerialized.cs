#nullable enable
namespace CCEnvs.UnityX.EditorSerialization
{
    public interface IEditorSerialized
    {
        object Deserialized { get; }
    }
    public interface IEditorSerialized<out T> : IEditorSerialized
    {
        new T Deserialized { get; }

        object IEditorSerialized.Deserialized => Deserialized!;
    }
}
