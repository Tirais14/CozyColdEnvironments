#nullable enable
namespace CCEnvs.Saves
{
    public interface ISaveObjectIncremental
    {
        event OnSaveObjectIsDirtyChanged OnSaveObjectIsDirtyChanged;

        bool IsSaveObjectDirty { get; }

        void MarkSaveObjectDirty();
    }
}
