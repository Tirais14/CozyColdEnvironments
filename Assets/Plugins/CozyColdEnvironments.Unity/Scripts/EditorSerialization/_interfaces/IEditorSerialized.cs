#nullable enable
namespace CCEnvs.UnityX.EditorSerialization
{
    public interface IEditorSerialized
    {
        object Data { get; }
    }
    public interface IEditorSerialized<out T> : IEditorSerialized
    {
        new T Data { get; }

        object IEditorSerialized.Data => Data!;
    }
}
