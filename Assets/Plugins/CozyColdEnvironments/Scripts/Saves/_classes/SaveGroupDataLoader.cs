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
        private readonly CommandScheduler commandScheduler = CommandScheduler.CreateDefaultRegistered(nameof(SaveGroupDataLoader));

        private ReactiveCommand<SaveData>? onLoadedSaveData;

        public SaveGroup Group { get; }

        public SaveData Data => Group.SaveData;

        public bool IsDataLoaded { get; private set; }

        public bool RedirectFromFileToSerializedStorage { get; }

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
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (RedirectFromFileToSerializedStorage
                &&
                TryGetSerializedSaveGroup(out var serializedGroup))
            {
                return await DeserializeSaveDataFromSerializedAsync(
                    serializedGroup.SaveDataSerialized,
                    configureAwait: configureAwait,
                    cancellationToken: cancellationToken
                    );
            }

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
                .ScheduleBy(commandScheduler)
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
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (RedirectFromFileToSerializedStorage
                &&
                TryGetSerializedSaveGroup(out var serializedGroup))
            {
                await LoadSaveDataFromSerializedAsync(
                    serializedGroup.SaveDataSerialized,
                    writeSaveDataMode: writeSaveDataMode,
                    configureAwait: configureAwait,
                    cancellationToken: cancellationToken
                    );
            }

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
                .ScheduleBy(commandScheduler)
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
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (forceGet || IsDataLoaded)
                return Data;

            await LoadSaveDataFromFileAsync(
                writeSaveDataMode,
                configureAwait,
                cancellationToken
                );

            return Data;
        }

        public async ValueTask<SaveData?> DeserializeSaveDataFromSerializedAsync(
            string serialized,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            var cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(DeserializeSaveDataFromSerializedAsync)
                );

            var result = new ValueReference<SaveData?>();

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, serialized, configureAwait, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.DeserializeSaveDataFromSerializedAsyncCore(
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
                .ScheduleBy(commandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif

            return result;
        }

        public async ValueTask LoadSaveDataFromSerializedAsync(
            string serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadSaveDataFromSerializedAsync)
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, serialized, configureAwait, writeSaveDataMode))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadSaveDataFromSerializedAsyncCore(
                        serialized: args.serialized,
                        writeSaveDataMode: args.writeSaveDataMode,
                        configureAwait: args.configureAwait,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(commandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif
        }

        public async ValueTask<SaveData> GetOrLoadSaveDataFromSerializedAsync(
            string serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            bool forceGet = false,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (forceGet || IsDataLoaded)
                return Data;

            await LoadSaveDataFromSerializedAsync(
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

            commandScheduler.Dispose();
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
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

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
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

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

        private async ValueTask<SaveData?> DeserializeSaveDataFromSerializedAsyncCore(
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

        private async ValueTask LoadSaveDataFromSerializedAsyncCore(
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
                var saveData = await DeserializeSaveDataFromSerializedAsyncCore(
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

        private bool TryGetSerializedSaveGroup(out SaveGroupSerialized serializedGroup)
        {
            serializedGroup = default;

            var archivePath = Group.Catalog.Archive.Path;

            if (!SaveSystemSerializedStorage.Archives.TryGetValue(archivePath, out var serializedArchive))
                return false;

            var catalogPath = Group.Catalog.Path;

            if (!serializedArchive.Catalogs.TryGetValue(catalogPath, out var serializedCatalog))
                return false;

            return serializedCatalog.Groups.TryGetValue(Group.Name, out serializedGroup);
        }
    }
}
