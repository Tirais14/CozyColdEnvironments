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
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (Catalog.Groups.IsEmpty())
                return;

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadGroupsFromFileAsync)
                );

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, writeSaveDataMode))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadGroupsFromFileAsyncCore(
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

        public async ValueTask LoadGroupsFromSerializedAsync(
            SaveCatalogSerialized serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (Catalog.Groups.IsEmpty())
                return;

            if (serialized == default)
                return;

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadGroupsFromSerializedAsync)
                );

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, serialized, writeSaveDataMode))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadGroupsFromSerializedAsyncCore(
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

        private async ValueTask LoadGroupsFromFileAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

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
                        task = group.Loader.LoadSaveDataFromFileAsync(
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

        private async ValueTask LoadGroupsFromSerializedAsyncCore(
            SaveCatalogSerialized serialized,
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

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

                    task = group.Loader.LoadSaveDataFromSerializedAsync(
                        serializedGroup.SaveDataSerialized,
                        writeSaveDataMode,
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
        }
    }
}
