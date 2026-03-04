using System.IO;
using CCEnvs.Saves;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Components;
using UnityEngine;

#nullable enable
namespace Core.Playables
{
    public class Player : CCBehaviour
    {
        private SaveGroupRegistration saveSysReg;

        private SaveGroupIncremental saveGroup = null!;

        public event OnSaveObjectIsDirtyChanged? OnSaveObjectIsDirtyChanged;

        [field: SerializeField]
        public float Health { get; set; } = 100f;

        [field: SerializeField]
        public float Drunkness { get; set; } = 0.1f;

        public bool IsSaveObjectDirty => throw new System.NotImplementedException();

        protected override void Awake()
        {
            base.Awake();

            SaveSystem.RegisterType(
                this,
                static (player) =>
                {
                    return new PlayerSnapshot(player);
                });

            saveGroup = SaveSystem.GetOrCreateArchive(G.SaveDefaultArchivePath)
                .GetOrCreateCatalog(G.SaveDefaultCatalogPath)
                .GetOrCreateIncrementalGroup(G.SaveDefaultGroupName);
        }

        public void RegisterInSaveSystem()
        {
            saveSysReg = saveGroup.RegisterObjectHandled(this);
        }

        public void LoadSaveGame()
        {
            saveGroup.LoadSaveDataFromFileAsync()
                .ForgetByPrintException();
        }

        public void SaveGame()
        {
            saveGroup.CaptureAndWriteSaveData()
                .WriteSaveDataToFileAsync(compressed: false)
                .ForgetByPrintException();
        }

        public void MarkSaveObjectDirty()
        {
            //OnSaveObjectIsDirtyChanged?.Invoke(this, true);
        }
    }
}
