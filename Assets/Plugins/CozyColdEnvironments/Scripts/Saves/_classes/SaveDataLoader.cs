using System;
using System.Threading;
using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CCEnvs.Saves;
using CCEnvs.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Humanizer;
using Newtonsoft.Json.Converters;
using R3;
using UnityEngine;

namespace CCEnvs
{
    public class SaveDataLoader
    {
        public SaveData Data { get; set; }

        public bool IsLoaded { get; private set; }

        public async UniTask LoadSaveDataFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            bool force = false,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            if (!force && IsLoaded)
                return;

            await UniTaskHelper.TrySwitchToThreadPool();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadSaveDataFromFileAsync),
                expirationTimeRelativeToNow: 2.Minutes()
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, configureAwait, writeSaveDataMode))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadSaveDataFromFileAsyncCore(
                        args.writeSaveDataMode,
                        args.configureAwait,
                        cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(SaveSystem.CommandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
        }

        private async UniTask LoadSaveDataFromFileAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            cancellationToken.ThrowIfCancellationRequested();

            var loadedSaveData = await GetSaveDataFromFileAsyncCore(
                configureAwait: false,
                cancellationToken
                );

            await UniTaskHelper.TrySwitchToThreadPool();

            try
            {
                if (loadedSaveData is null)
                {
                    Data.Write(Array.Empty<SaveEntry>(), writeSaveDataMode);

                    IsDataLoadedFromFile = true;

                    return;
                }

                SaveData.Write(loadedSaveData.SaveEntries.Values, writeSaveDataMode);

                IsDataLoadedFromFile = true;
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                return;
            }
            finally
            {
                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
        }
    }
}
