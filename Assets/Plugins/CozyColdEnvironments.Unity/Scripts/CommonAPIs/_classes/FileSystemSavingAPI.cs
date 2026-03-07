using System.Threading;
using CCEnvs.Attributes;
using CCEnvs.Dependencies;
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
using R3;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    public sealed class FileSystemSavingAPI : ISavingAPI
    {
        [field: OnInstallResetable]
        public static FileSystemSavingAPI? Instance { get; private set; }

        public bool IsGameSaving => SavingSystem.Self.IsSaving;
        public bool IsSaveGameLoading => SavingSystem.Self.IsSaveLoading;

        public FileSystemSavingAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(DefaultGeneralAPI));

            Instance = this;

            CCDependecyContainer.Bind<ISavingAPI>(this);
            CCDependecyContainer.Bind(this);
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

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            CCDependecyContainer.Unbind<ISavingAPI>();
            CCDependecyContainer.Unbind(GetType());

            disposed = true;
        }

        public Observable<bool> ObserveGameSaving()
        {
            return SavingSystem.Self.ObserveSaving();
        }

        public Observable<bool> ObserveSaveGameLoading()
        {
            return SavingSystem.Self.ObserveSaveLoading();
        }
    }
}
