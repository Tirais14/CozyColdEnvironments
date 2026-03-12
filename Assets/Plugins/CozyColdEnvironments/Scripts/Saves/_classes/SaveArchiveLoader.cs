using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
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
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (Archive.Catalogs.IsEmpty())
                return;

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadCatalogsFromFileAsync),
                expirationTimeRelativeToNow: TimeSpan.Zero
                );

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, writeSaveDataMode))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadCatalogsFromFileAsyncCore(
                        writeSaveDataMode: args.writeSaveDataMode,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(commandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);
        }

        public async ValueTask LoadCatalogsFromSerializedAsync(
            SaveArchiveSerialized serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (Archive.Catalogs.IsEmpty())
                return;

            if (serialized == default)
                return;

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadCatalogsFromSerializedAsync),
                expirationTimeRelativeToNow: TimeSpan.Zero
                );

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, serialized, writeSaveDataMode))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadCatalogsFromSerializedAsyncCore(
                        serialized: args.serialized,
                        writeSaveDataMode: args.writeSaveDataMode,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(commandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);
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
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

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
                            cancellationToken: cancellationToken
                            );

                        tasks.Value.Add(task);
                    }
                }

                await ValueTaskEx.WhenAll(tasks.Value);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return;
            }
        }

        private async ValueTask LoadCatalogsFromSerializedAsyncCore(
            SaveArchiveSerialized serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

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
                        cancellationToken: cancellationToken
                        );

                    tasks.Value.Add(task);
                }

                await ValueTaskEx.WhenAll(tasks.Value);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return;
            }
        }
    }
}
