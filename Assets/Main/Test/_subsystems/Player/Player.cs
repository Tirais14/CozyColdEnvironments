using System.IO;
using CCEnvs.Saves;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Components;
using UnityEngine;

#nullable enable
namespace Tests.SubSystems.Players
{
    public class Player : CCBehaviour
    {
        private SaveGroupRegistration saveSysReg;

        [field: SerializeField]
        public float Health { get; set; } = 100f;

        [field: SerializeField]
        public float Drunkness { get; set; } = 0.1f;

        protected override void Awake()
        {
            base.Awake();

            SaveSystem.RegisterType(
                this,
                static (player) =>
                {
                    return new PlayerSnapshot(player);
                });
        }

        public void RegisterInSaveSystem()
        {
            saveSysReg = SaveSystem.GetOrCreateArchive(Path.Join(Application.dataPath, "Save1"))
                .GetOrCreateCatalog()
                .GetOrCreateIncrementalGroup("PlayerInfo")
                .RegisterObjectHandled(this);
        }

        public void LoadSaveGame()
        {
            SaveSystem.GetOrCreateArchive(Path.Join(Application.dataPath, "Save1"))
                .GetOrCreateCatalog()
                .GetOrCreateIncrementalGroup("PlayerInfo")
                .LoadSaveDataFromFileAsync()
                .ForgetByPrintException();
        }

        public void SaveGame()
        {
            SaveSystem.GetOrCreateArchive(Path.Join(Application.dataPath, "Save1"))
                .GetOrCreateCatalog()
                .GetOrCreateIncrementalGroup("PlayerInfo")
                .CaptureAndWriteSaveData()
                .WriteSaveDataToFileAsync()
                .ForgetByPrintException();
        }
    }
}
