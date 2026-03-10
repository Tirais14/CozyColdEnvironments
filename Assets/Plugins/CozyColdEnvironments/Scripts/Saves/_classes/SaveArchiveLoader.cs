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
    public sealed class SaveArchiveLoader : IDisposable
    {
        private readonly CommandScheduler commandScheduler = CommandScheduler.CreateDefaultRegistered();

        public SaveArchive Archive { get; }

        public SaveArchiveLoader(SaveArchive archive)
        {
            Guard.IsNotNull(archive, nameof(archive));

            Archive = archive;
        }

        ~SaveArchiveLoader() => Dispose();

        public async ValueTask LoadCatalogsFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            if (Archive.Catalogs.IsEmpty())
                return;

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadCatalogsFromFileAsync),
                expirationTimeRelativeToNow: TimeSpan.Zero
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, writeSaveDataMode, configureAwait))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadCatalogsFromFileAsyncCore(
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

        public async ValueTask LoadCatalogsFromSerializedAsync(
            SaveArchiveSerialized serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            if (Archive.Catalogs.IsEmpty())
                return;

            if (serialized == default)
                return;

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadCatalogsFromSerializedAsync),
                expirationTimeRelativeToNow: TimeSpan.Zero
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, serialized, writeSaveDataMode, configureAwait))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadCatalogsFromSerializedAsyncCore(
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

        private async ValueTask LoadCatalogsFromFileAsyncCore(
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
            tasks.Value.TryIncreaseCapacity(Archive.Catalogs.Count);
#pragma warning restore CS4014

            ValueTask task;

            try
            {
                lock (Archive.Catalogs.SyncRoot)
                {
                    foreach (var (_, catalog) in Archive.Catalogs)
                    {
                        task = catalog.Loader.LoadGroupsFromFileAsync(
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

        private async ValueTask LoadCatalogsFromSerializedAsyncCore(
            SaveArchiveSerialized serialized,
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
            tasks.Value.TryIncreaseCapacity(Archive.Catalogs.Count);
#pragma warning restore CS4014

            ValueTask task;

            try
            {
                foreach (var (_, serializedCatalog) in serialized.Catalogs)
                {
                    if (!Archive.Catalogs.TryGetValue(serializedCatalog.Path, out var catalog))
                        continue;

                    task = catalog.Loader.LoadGroupsFromSerializedAsync(
                        serializedCatalog,
                        writeSaveDataMode: writeSaveDataMode,
                        configureAwait: configureAwait,
                        cancellationToken: cancellationToken
                        );

                    tasks.Value.Add(task);
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
    }
}
