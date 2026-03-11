using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;
using ValueTaskSupplement;

#nullable enable
namespace CCEnvs.Saves
{
    public sealed class SaveCatalogSerializer : IDisposable
    {
        private readonly CommandScheduler commandScheduler = CommandScheduler.CreateDefaultRegistered();

        public SaveCatalog Catalog { get; }

        public SaveCatalogSerializer(SaveCatalog catalog)
        {
            Guard.IsNotNull(catalog, nameof(catalog));

            Catalog = catalog;
        }

        ~SaveCatalogSerializer() => Dispose();

        public async ValueTask SerializeGroupsToFileAsync(
            SerializeToFileParameters parameters = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeGroupsToFileAsync)
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, parameters, configureAwait))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.SerializeGroupsToFileAsyncCore(
                        parameters: args.parameters,
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
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
        }

        public async ValueTask<SaveCatalogSerialized> SerializeCatalogAsync(
            bool compressed = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeCatalogAsync)
                );

            var result = new ValueReference<SaveCatalogSerialized>();

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, compressed, configureAwait, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.SerializeCatalogAsyncCore(
                        compressed: args.compressed,
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
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif

            return result;
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            commandScheduler.Dispose();
        }

        private async ValueTask SerializeGroupsToFileAsyncCore(
            SerializeToFileParameters parameters = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToThreadPool();
#endif

            var tasks = ListPool<ValueTask>.Shared.Get();

            ValueTask task;

            try
            {
                lock (Catalog.Groups.SyncRoot)
                {
                    foreach (var (_, group) in Catalog.Groups)
                    {
                        task = group.Serializer.SerializeDataToFileAsync(
                            parameters: parameters,
                            configureAwait: false,
                            cancellationToken
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
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            finally
            {
                await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
#endif
        }

        private async ValueTask<SaveCatalogSerialized> SerializeCatalogAsyncCore(
            bool compressed = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToThreadPool();
#endif

            try
            {
                var serializeTasks = ListPool<ValueTask<SaveGroupSerialized>>.Shared.Get();

                ValueTask<SaveGroupSerialized> serializeTask;

                foreach (var (_, group) in Catalog.Groups)
                {
                    serializeTask = group.Serializer.SerializeGroupAsync(
                        compressed: compressed,
                        configureAwait: false,
                        cancellationToken: cancellationToken
                        );

                    serializeTasks.Value.Add(serializeTask);
                }

                var serializedGroups = await ValueTaskEx.WhenAll(serializeTasks.Value);

                return new SaveCatalogSerialized(Catalog.Path, serializedGroups);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return new SaveCatalogSerialized(Catalog.Path);
            }
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            finally
            {
                await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
#endif
        }
    }
}
