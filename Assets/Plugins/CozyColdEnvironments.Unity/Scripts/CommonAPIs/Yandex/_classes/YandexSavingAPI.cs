#if PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.Dependencies;
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
using R3;
using System.Threading;
using YG;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs.Yandex
{
    public class YandexSavingAPI : ISavingAPI
    {
        [field: OnInstallResetable]
        public static YandexSavingAPI? Instance { get; private set; }

        public bool IsGameSaving => SavingSystem.Self.IsSaving;
        public bool IsSaveGameLoading => SavingSystem.Self.IsSaveLoading;

        public YandexSavingAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexSavingAPI));

            Instance = this;
            BuiltInDependecyContainer.BindTo<ISavingAPI>(this);
            BuiltInDependecyContainer.BindTo(this);
        }

        public async UniTask SaveGameAsync(
            string? filePath = null,
            CancellationToken cancellationToken = default
            )
        {
            await SavingSystem.Self.SaveInMemoryAsync(cancellationToken);
        }

        public async UniTask LoadSaveGameAsync(
            string? filePath = null,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                await SavingSystem.Self.LoadFromSerializedData(
                    YG2.saves.serializedData,
                    cancellationToken
                    );
            }
            catch (System.Exception ex)
            {
                this.PrintException(ex);
            }
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
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
#endif //PLUGIN_YG_2
