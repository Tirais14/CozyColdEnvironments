using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;
using ValueTaskSupplement;

#nullable enable
namespace CCEnvs.Saves
{
    public sealed class SaveCatalogLoader : IDisposable
    {
        private readonly CommandScheduler commandScheduler = new(ObservableSystem.DefaultFrameProvider, nameof(SaveCatalogLoader));

        public SaveCatalog Catalog { get; }

        public SaveCatalogLoader(SaveCatalog catalog)
        {
            Guard.IsNotNull(catalog, nameof(catalog));

            Catalog = catalog;
        }

        ~SaveCatalogLoader() => Dispose();

        public async ValueTask LoadGroupsFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            if (Catalog.Groups.IsEmpty())
                return;

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadGroupsFromFileAsync)
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, writeSaveDataMode, configureAwait))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadGroupsFromFileAsyncCore(
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
            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
        }

        public async ValueTask LoadGroupsFromSerializedAsync(
            SaveCatalogSerialized serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            if (Catalog.Groups.IsEmpty())
                return;

            if (serialized == default)
                return;

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadGroupsFromSerializedAsync)
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, serialized, writeSaveDataMode, configureAwait))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadGroupsFromSerializedAsyncCore(
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
            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            commandScheduler.Dispose();
        }

        private async ValueTask LoadGroupsFromFileAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            using var tasks = ListPool<ValueTask>.Shared.Get();

#pragma warning disable CS4014
            tasks.Value.TryIncreaseCapacity(Catalog.Groups.Count);
#pragma warning restore CS4014

            ValueTask task;

            try
            {
                lock (Catalog.Groups.SyncRoot)
                {
                    foreach (var (_, group) in Catalog.Groups)
                    {
                        task = group.SaveDataLoader.LoadSaveDataFromFileAsync(
                            writeSaveDataMode,
                            configureAwait: false,
                            cancellationToken: cancellationToken
                            );

                        tasks.Value.Add(task);
                    }
                }

                await ValueTaskEx.WhenAll(tasks.Value);
            }
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            finally
            {
                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
#endif
        }

        private async ValueTask LoadGroupsFromSerializedAsyncCore(
            SaveCatalogSerialized serialized,
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

            using var tasks = ListPool<ValueTask>.Shared.Get();

#pragma warning disable CS4014
            tasks.Value.TryIncreaseCapacity(Catalog.Groups.Count);
#pragma warning restore CS4014

            ValueTask task;

            try
            {
                foreach (var (_, serializedGroup) in serialized.Groups)
                {
                    if (!Catalog.Groups.TryGetValue(serializedGroup.Name, out var group))
                        continue;

                    task = group.SaveDataLoader.LoadSaveDataFromSerializedAsync(
                        serializedGroup.SaveDataSerialized,
                        writeSaveDataMode,
                        configureAwait: false,
                        cancellationToken: cancellationToken
                        );

                    tasks.Value.Add(task);
                }
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return;
            }
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            finally
            {
                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
#endif
        }
    }
}
