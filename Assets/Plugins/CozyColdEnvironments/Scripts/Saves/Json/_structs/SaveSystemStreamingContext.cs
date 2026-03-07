#nullable enable
namespace CCEnvs.Saves.Json
{
    internal sealed class SaveSystemStreamingContext
    {
        public long SaveDataVersion { get; }

        public SaveSystemStreamingContext(long saveDataVersion)
        {
            SaveDataVersion = saveDataVersion;
        }
    }
}
