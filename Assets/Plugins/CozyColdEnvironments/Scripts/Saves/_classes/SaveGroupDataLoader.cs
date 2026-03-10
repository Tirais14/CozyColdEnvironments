using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Saves
{
    public sealed class SaveGroupDataLoader : IDisposable
    {
        private ReactiveCommand<SaveData>? onLoadedSaveData;

        public SaveGroup Group { get; }

        public SaveData Data => Group.SaveData;

        public bool IsDataLoaded { get; private set; }

        public SaveGroupDataLoader(SaveGroup group)
        {
            Guard.IsNotNull(group, nameof(group));

            Group = group;
        }

        ~SaveGroupDataLoader() => Dispose();

        public async ValueTask<SaveData?> DeserializeSaveDataFromFileAsync(
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(DeserializeSaveDataFromFileAsync)
                );

            var result = new ValueReference<SaveData?>();

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, configureAwait, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.DeserializeSaveDataFromFileAsyncCore(
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

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif

            return result;
        }

        public async ValueTask LoadSaveDataFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            bool force = false,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            if (!force && IsDataLoaded)
                return;

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadSaveDataFromFileAsync)
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

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
        }

        public async ValueTask<SaveData> GetOrLoadSaveDataFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            bool forceGet = false,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            if (forceGet || IsDataLoaded)
                return Data;

            await LoadSaveDataFromFileAsync(
                writeSaveDataMode,
                configureAwait,
                forceGet,
                cancellationToken
                );

            return Data;
        }

        public async ValueTask<SaveData?> DeserializeSaveDataTextAsync(
            string serialized,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            var cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(DeserializeSaveDataTextAsync)
                );

            var result = new ValueReference<SaveData?>();

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, serialized, configureAwait, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.DeserializeSaveDataFromTextAsyncCore(
                        serialized: args.serialized,
                        configureAwait: args.configureAwait,
                        cancellationToken: cancellationToken
                        );

#if !PLATFORM_WEBGL && !UNITASK_PLUGIN
                    await Task.Run(async () =>
                    {
                        args.result = await args.@this.DeserializeSaveDataAsyncCore(
                            serialized: args.serialized,
                            configureAwait: args.configureAwait,
                            cancellationToken: cancellationToken
                            );
                    },
                    cancellationToken)
                    .ConfigureAwait(args.configureAwait);
#endif
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(SaveSystem.CommandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif

            return result;
        }

        public async ValueTask LoadSaveDataFromTextAsync(
            string serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadSaveDataFromTextAsync)
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, serialized, configureAwait, writeSaveDataMode))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadSaveDataFromTextAsyncCore(
                        serialized: args.serialized,
                        writeSaveDataMode: args.writeSaveDataMode,
                        configureAwait: args.configureAwait,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(SaveSystem.CommandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif
        }

        public async ValueTask<SaveData> GetOrLoadSaveDataFromTextAsync(
            string serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            bool forceGet = false,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            if (forceGet || IsDataLoaded)
                return Data;

            await LoadSaveDataFromTextAsync(
                serialized: serialized,
                writeSaveDataMode: writeSaveDataMode,
                configureAwait: configureAwait,
                cancellationToken: cancellationToken
                );

            return Data;
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            onLoadedSaveData?.Dispose();
        }

        public Observable<SaveData> ObserveLoadSaveData()
        {
            onLoadedSaveData ??= new ReactiveCommand<SaveData>();

            return onLoadedSaveData;
        }

        private async ValueTask<SaveData?> DeserializeSaveDataFromFileAsyncCore(
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            cancellationToken.ThrowIfCancellationRequested();

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            var filePath = Group.GetFullPath();

            try
            {
                var loadedSaveData = await SaveLoad.DataFromFileAsync(
                    filePath,
                    configureAwait: false,
                    cancellationToken
                    );

                return loadedSaveData;
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                return null;
            }
            finally
            {
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
            }
        }

        private async ValueTask LoadSaveDataFromFileAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            var loadedSaveData = await DeserializeSaveDataFromFileAsyncCore(
                configureAwait: false,
                cancellationToken
                );

#if !PLATFORM_WEBGL
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            try
            {
                if (loadedSaveData is null)
                {
                    Data.Write(Array.Empty<SaveEntry>(), writeSaveDataMode);

                    IsDataLoaded = true;

                    return;
                }

                Data.Write(loadedSaveData.SaveEntries.Values, writeSaveDataMode);

                IsDataLoaded = true;

                onLoadedSaveData?.Execute(Data);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                return;
            }
            finally
            {
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
            }
        }

        private async ValueTask<SaveData?> DeserializeSaveDataFromTextAsyncCore(
            string serialized,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            cancellationToken.ThrowIfCancellationRequested();

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            try
            {
                return await SaveSerializer.DeserializeAsync(serialized, cancellationToken);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                return null;
            }
            finally
            {
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
            }
        }

        private async ValueTask LoadSaveDataFromTextAsyncCore(
            string serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            try
            {
                var saveData = await DeserializeSaveDataFromTextAsyncCore(
                    serialized,
                    configureAwait: false,
                    cancellationToken
                    );

                var entries = saveData?.SaveEntries.SelectValue() ?? Array.Empty<SaveEntry>();

                Data.Write(entries, writeSaveDataMode: writeSaveDataMode);

                onLoadedSaveData?.Execute(Data);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return;
            }
            finally
            {
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
            }
        }
    }
}
