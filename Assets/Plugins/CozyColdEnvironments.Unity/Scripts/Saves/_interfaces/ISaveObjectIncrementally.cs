#nullable enable
namespace CCEnvs.Unity.Saves
{
    public interface ISaveObjectIncrementally
    {
        event OnSaveObjectIsDirtyChanged OnSaveObjectIsDirtyChanged;

        bool IsSaveObjectDirty { get; }

        void MarkSaveObjectDirty();
    }
}
