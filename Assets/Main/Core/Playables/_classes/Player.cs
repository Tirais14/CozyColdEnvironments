using CCEnvs.Saves;
using CCEnvs.Threading;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using System.Threading;
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

            saveGroup = Saves.GetPlayerDataGroup();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            saveSysReg.Dispose();
        }

        public void RegisterInSaveSystem()
        {
            saveSysReg = saveGroup.RegisterObjectHandled(this);
        }

        public void LoadSaveGame()
        {
            saveGroup.Loader.LoadSaveDataFromFileAsync(cancellationToken: destroyCancellationToken)
                .AsUniTask()
                .ForgetByPrintException();
        }

        public void SavePlayerData()
        {
            SavePlayerDataAsync().ForgetByPrintException();
        }

        public async UniTask SavePlayerDataAsync(CancellationToken cancellationToken = default)
        {
            using var _ = destroyCancellationToken.TryLinkTokens(cancellationToken, out cancellationToken);

            await saveGroup.Writer.CaptureAndWriteSaveDataAsync(cancellationToken: cancellationToken);

            await saveGroup.Serializer.SerializeDataToFileAsync(
                Saves.DefaultSerializeToFileParams,
                cancellationToken: cancellationToken
                );
        }

        public void MarkSaveObjectDirty()
        {
            //OnSaveObjectIsDirtyChanged?.Invoke(this, true);
        }
    }
}
