#if PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.Dependencies;
using CCEnvs.Threading;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using YG;

#nullable enable
#pragma warning disable S2930
namespace CCEnvs.Unity.CommonAPIs.Yandex
{
    public class YandexSavingAPI : ISavingAPI
    {
        [field: OnInstallResetable]
        public static YandexSavingAPI? Instance { get; private set; }

        private readonly IGeneralAPI generalAPI;

        private readonly List<IDisposable> disposables = new(2);

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        public bool IsGameSaving => SavingSystem.Self.IsSaving;
        public bool IsSaveGameLoading => SavingSystem.Self.IsSaveLoading;

        public YandexSavingAPI(IGeneralAPI generalAPI)
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexSavingAPI));

            this.generalAPI = generalAPI;

            BindSavingSystem();
            BindGeneralAPI();

            Instance = this;

            CCDependecyContainer.Bind<ISavingAPI>(this);
            CCDependecyContainer.Bind(this);
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
            cancellationToken.ThrowIfCancellationRequested();

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

            YG2.onDefaultSaves -= OnSetDefaultSaves;

            disposeCancellationTokenSource.CancelAndDispose();
            disposables.DisposeEachAndClear(bufferized: false);

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

        private void OnSetDefaultSaves()
        {
            YG2.saves.serializedData = string.Empty;
        }

        private void BindSavingSystem()
        {
            YG2.onDefaultSaves += OnSetDefaultSaves;

            SavingSystem.Self.ObserveSaveData()
                .Where(this, static (_, @this) => @this.generalAPI.IsInitialized)
                .Subscribe(this,
                static (saveData, @this) =>
                {
                    var jSettings = CC.JsonSettings;

                    jSettings.Formatting = Formatting.None;

                    var serializedSaveData = JsonConvert.SerializeObject(saveData, jSettings);

                    YG2.saves.serializedData = serializedSaveData;

                    YG2.SaveProgress();
                })
                .AddTo(disposables);
        }

        private void BindGeneralAPI()
        {
            generalAPI.ObserveIsInitialized()
                .Where(static state => state)
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.LoadSaveGameAsync(cancellationToken: @this.disposeCancellationTokenSource.Token)
                        .ForgetByPrintException();
                })
                .AddTo(disposables);
        }
    }
}
#endif //PLUGIN_YG_2
