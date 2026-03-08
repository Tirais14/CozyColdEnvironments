#nullable enable
namespace CCEnvs
{
//    public class SaveDataLoader
//    {
//        public SaveData Data { get; set; }

//        public bool IsLoaded { get; private set; }

//        public async UniTask LoadSaveDataFromFileAsync(
//            WriteSaveDataMode writeSaveDataMode = default,
//            bool configureAwait = true,
//            bool force = false,
//            CancellationToken cancellationToken = default
//            )
//        {
//            cancellationToken.ThrowIfCancellationRequested();

//            if (!force && IsLoaded)
//                return;

//#if !PLATFORM_WEBGL
//            await UniTaskHelper.TrySwitchToThreadPool();
//#endif

//            string cmdName = NameFactory.CreateFromCaller(
//                this,
//                nameof(LoadSaveDataFromFileAsync),
//                expirationTimeRelativeToNow: 2.Minutes()
//                );

//            await Command.Builder.WithName(cmdName)
//                .WithState((@this: this, configureAwait, writeSaveDataMode))
//                .Asynchronously()
//                .WithExecuteAction(
//                static async (args, cancellationToken) =>
//                {
//                    await args.@this.LoadSaveDataFromFileAsyncCore(
//                        args.writeSaveDataMode,
//                        args.configureAwait,
//                        cancellationToken
//                        );
//                })
//                .BuildPooled()
//                .Value
//                .AttachExternalCancellationToken(cancellationToken)
//                .ScheduleBy(SaveSystem.CommandScheduler)
//                .ObserveIsDone()
//                .FirstAsync(cancellationToken);

//            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
//        }

//        private async UniTask LoadSaveDataFromFileAsyncCore(
//            WriteSaveDataMode writeSaveDataMode = default,
//            bool configureAwait = true,
//            CancellationToken cancellationToken = default
//            )
//        {
//            cancellationToken.ThrowIfCancellationRequested();

//            var loadedSaveData = await GetSaveDataFromFileAsyncCore(
//                configureAwait: false,
//                cancellationToken
//                );

//#if !PLATFORM_WEBGL
//            await UniTaskHelper.TrySwitchToThreadPool();
//#endif

//            try
//            {
//                if (loadedSaveData is null)
//                {
//                    Data.Write(Array.Empty<SaveEntry>(), writeSaveDataMode);

//                    IsLoaded = true;

//                    return;
//                }

//                Data.Write(loadedSaveData.SaveEntries.Values, writeSaveDataMode);

//                IsLoaded = true;
//            }
//            catch (Exception ex)
//            {
//                this.PrintException(ex);

//                return;
//            }
//            finally
//            {
//                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
//            }
//        }

//        private async UniTask<SaveData?> GetSaveDataFromFileAsyncCore(
//            string path,
//            bool configureAwait = true,
//            CancellationToken cancellationToken = default
//            )
//        {
//            cancellationToken.ThrowIfCancellationRequested();

//#if !PLATFORM_WEBGL
//            await UniTaskHelper.TrySwitchToThreadPool();
//#endif

//            var filePath = GetFullPath();

//            try
//            {
//                var loadedSaveData = await SaveLoad.DataFromFileAsync(
//                    filePath,
//                    configureAwait: false,
//                    cancellationToken
//                    );

//                return loadedSaveData;
//            }
//            catch (Exception ex)
//            {
//                this.PrintException(ex);

//                return null;
//            }
//            finally
//            {
//                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
//            }
//        }
//    }
}
