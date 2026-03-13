using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Saves
{
    public sealed class SaveGroupLoader : IDisposable
    {
        private ReactiveCommand<SaveData>? onLoaded;

        public SaveGroup Group { get; }

        public SaveData Data => Group.SaveData;

        public bool IsDataLoaded { get; private set; }

        public RedirectionMode Redirection => Group.Redirection;

        public SaveGroupLoader(
            SaveGroup group
            )
        {
            Guard.IsNotNull(group, nameof(group));

            Group = group;
        }

        ~SaveGroupLoader() => Dispose();

        public async ValueTask<SaveData?> DeserializeSaveDataFromFileAsync(
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (Redirection == RedirectionMode.FromFileToSerializedStorage)
            {
                if (!TryGetSerializedSaveGroup(out var serializedGroup))
                    return null; 

                return await DeserializeSaveDataFromSerializedAsync(
                    serializedGroup.SaveDataSerialized,
                    cancellationToken: cancellationToken
                    );
            }

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(DeserializeSaveDataFromFileAsync)
                );

            using var result = ValueReferencePool<SaveData?>.Shared.Get();

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, result: result.Value))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result.Value = await args.@this.DeserializeSaveDataFromFileAsyncCore(
                        cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(Group.commandScheduler)
                .WaitForDone();

            return result.Value;
        }

        public async ValueTask LoadSaveDataFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (Redirection == RedirectionMode.FromFileToSerializedStorage)
            {
                if (!TryGetSerializedSaveGroup(out var serializedGroup))
                    return;

                await LoadSaveDataFromSerializedAsync(
                    serializedGroup.SaveDataSerialized,
                    writeSaveDataMode: writeSaveDataMode,
                    cancellationToken: cancellationToken
                    );
            }

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadSaveDataFromFileAsync)
                );

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, writeSaveDataMode))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadSaveDataFromFileAsyncCore(
                        writeSaveDataMode: args.writeSaveDataMode,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(Group.commandScheduler)
                .WaitForDone();
        }

        public async ValueTask<SaveData> GetOrLoadSaveDataFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            bool forceGet = false,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (forceGet || IsDataLoaded)
                return Data;

            await LoadSaveDataFromFileAsync(
                writeSaveDataMode,
                cancellationToken
                );

            return Data;
        }

        public async ValueTask<SaveData?> DeserializeSaveDataFromSerializedAsync(
            string serialized,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            var cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(DeserializeSaveDataFromSerializedAsync)
                );

            using var result = ValueReferencePool<SaveData?>.Shared.Get();

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, serialized, result: result.Value))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result.Value = await args.@this.DeserializeSaveDataFromSerializedAsyncCore(
                        serialized: args.serialized,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(Group.commandScheduler)
                .WaitForDone();

            return result.Value;
        }

        public async ValueTask LoadSaveDataFromSerializedAsync(
            string serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadSaveDataFromSerializedAsync)
                );

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, serialized, writeSaveDataMode))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadSaveDataFromSerializedAsyncCore(
                        serialized: args.serialized,
                        writeSaveDataMode: args.writeSaveDataMode,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(Group.commandScheduler)
                .WaitForDone();
        }

        public async ValueTask<SaveData> GetOrLoadSaveDataFromSerializedAsync(
            string serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            bool forceGet = false,
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
                cancellationToken: cancellationToken
                );

            return Data;
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            onLoaded?.Dispose();

            GC.SuppressFinalize(this);
        }

        public Observable<SaveData> ObserveLoadSaveData()
        {
            onLoaded ??= new ReactiveCommand<SaveData>();

            return onLoaded;
        }

        private async ValueTask<SaveData?> DeserializeSaveDataFromFileAsyncCore(
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

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
        }

        private async ValueTask LoadSaveDataFromFileAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            var loadedSaveData = await DeserializeSaveDataFromFileAsyncCore(
                cancellationToken
                );

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

                onLoaded?.Execute(Data);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                return;
            }
        }

        private async ValueTask<SaveData?> DeserializeSaveDataFromSerializedAsyncCore(
            string serialized,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            try
            {
                return await SaveSerializer.DeserializeAsync(serialized, cancellationToken);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                return null;
            }
        }

        private async ValueTask LoadSaveDataFromSerializedAsyncCore(
            string serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            try
            {
                var saveData = await DeserializeSaveDataFromSerializedAsyncCore(
                    serialized,
                    cancellationToken
                    );

                var entries = saveData?.SaveEntries.SelectValue() ?? Array.Empty<SaveEntry>();

                Data.Write(entries, writeSaveDataMode: writeSaveDataMode);

                onLoaded?.Execute(Data);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return;
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
