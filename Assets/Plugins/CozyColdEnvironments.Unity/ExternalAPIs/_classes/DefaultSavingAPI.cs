using CCEnvs.Attributes;
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public sealed class DefaultSavingAPI : ISavingAPI
    {
        [field: OnInstallResetable]
        public static DefaultSavingAPI? Instance { get; private set; }

        public bool IsGameSaving => SavingSystem.Self.IsSaving;
        public bool IsSaveGameLoading => SavingSystem.Self.IsSaveLoading;

        public DefaultSavingAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(DefaultGeneralAPI));

            Instance = this;
        }

        public async UniTask SaveGameAsync(
            string? filePath = null,
            CancellationToken cancellationToken = default
            )
        {
            if (filePath.IsNullOrWhiteSpace())
            {
                await SavingSystem.Self.SaveInMemoryAsync(cancellationToken);
                return;
            }

            await SavingSystem.Self.SaveInFileAsync(filePath, cancellationToken);
        }

        public async UniTask LoadSaveGameAsync(
            string? filePath = null,
            CancellationToken cancellationToken = default
            )
        {
            if (filePath.IsNullOrWhiteSpace())
                throw new System.NotSupportedException($"Save game loading without {nameof(filePath)} not supported");

            await SavingSystem.Self.LoadFromFileAsync(filePath, cancellationToken);
        }

        public void Dispose()
        {
        }

        public Observable<bool> ObserveGameSaving()
        {
            return SavingSystem.Self.ObserveSaving();
        }

        public Observable<bool> ObserveGameSaved()
        {
            return SavingSystem.Self.ObserveSaved();
        }

        public Observable<bool> ObserveSaveGameLoading()
        {
            return SavingSystem.Self.ObserveSaveLoading();
        }

        public Observable<bool> ObserveSaveGameLoaded()
        {
            return SavingSystem.Self.ObserveSaveLoaded();
        }
    }
}
