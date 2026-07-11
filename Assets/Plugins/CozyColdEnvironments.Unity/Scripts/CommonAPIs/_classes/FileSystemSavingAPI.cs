using CCEnvs.Services;
using CCEnvs.UnityX.Saves;
using Cysharp.Threading.Tasks;
using R3;
using System.Threading;

#nullable enable
namespace CCEnvs.UnityX.CommonAPIs
{
    public sealed class FileSystemSavingAPI : ISavingAPI
    {
        public static FileSystemSavingAPI? Instance { get; private set; }

        public bool IsGameSaving => SavingSystem.Self.IsSaving;
        public bool IsSaveGameLoading => SavingSystem.Self.IsSaveLoading;

        public FileSystemSavingAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(DefaultGeneralAPI));

            Instance = this;

            CCServices.BindInstance(this)
                .WithInterfaces()
                .AsSingle();
        }

        private static void OnInstall()
        {
            Instance?.Dispose();
            Instance = null;
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

            CCServices.Unbind<ISavingAPI>();
            CCServices.Unbind(GetType());

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
