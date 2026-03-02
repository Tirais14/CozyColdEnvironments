#nullable enable
namespace CCEnvs.Unity.Saves
{
    public interface ISaveObjectIncremental
    {
        event OnSaveObjectIsDirtyChanged OnSaveObjectIsDirtyChanged;

        bool IsSaveObjectDirty { get; }

        void MarkSaveObjectDirty();
    }
}
