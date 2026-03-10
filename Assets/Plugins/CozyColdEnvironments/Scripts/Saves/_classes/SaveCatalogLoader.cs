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
    public class SaveCatalogLoader : IDisposable
    {
        public SaveCatalog Catalog { get; }

        public SaveCatalogLoader(SaveCatalog catalog)
        {
            Guard.IsNotNull(catalog, nameof(catalog));

            Catalog = catalog;
        }

        ~SaveCatalogLoader() => Dispose();

        public async ValueTask LoadGroupsFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            bool force = false,
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
                .WithState((@this: this, writeSaveDataMode, configureAwait, force))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadGroupsFromFileAsyncCore(
                        args.writeSaveDataMode,
                        args.force,
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

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;
        }

        private async ValueTask LoadGroupsFromFileAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            bool force = false,
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
                            force,
                            cancellationToken: cancellationToken
                            );

                        tasks.Value.Add(task);
                    }
                }

                await ValueTaskEx.WhenAll(tasks.Value);
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
