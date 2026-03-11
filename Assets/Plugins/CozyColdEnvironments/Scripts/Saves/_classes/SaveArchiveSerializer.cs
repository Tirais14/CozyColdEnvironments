using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ValueTaskSupplement;

#nullable enable
namespace CCEnvs.Saves
{
    public class SaveArchiveSerializer : IDisposable
    {
        private readonly CommandScheduler commandScheduler = CommandScheduler.CreateDefaultRegistered();

        public SaveArchive Archive { get; }

        SaveArchiveSerializer(SaveArchive archive)
        {
            Guard.IsNotNull(archive, nameof(archive));

            Archive = archive;
        }

        ~SaveArchiveSerializer() => Dispose();

        public async ValueTask SerializeCatalogsToFileAsync(
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
                nameof(SerializeCatalogsToFileAsync)
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, parameters, configureAwait))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.SerializeCatalogsToFileAsyncCore(
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

        public async ValueTask<SaveArchiveSerialized> SerializeArchiveAsync(
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
                nameof(SerializeArchiveAsync)
                );

            var result = new ValueReference<SaveArchiveSerialized>();

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, compressed, configureAwait, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.SerializeArchiveAsyncCore(
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

        private async ValueTask SerializeCatalogsToFileAsyncCore(
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
                lock (Archive.Catalogs.SyncRoot)
                {
                    foreach (var (_, catalog) in Archive.Catalogs)
                    {
                        task = catalog.Serializer.SerializeGroupsToFileAsync(
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

        private async ValueTask<SaveArchiveSerialized> SerializeArchiveAsyncCore(
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

            var serializeTasks = ListPool<ValueTask<SaveCatalogSerialized>>.Shared.Get();

            ValueTask<SaveCatalogSerialized> serializeTask;

            try
            {
                lock (Archive.Catalogs.SyncRoot)
                {
                    foreach (var (_, catalog) in Archive.Catalogs)
                    {
                        serializeTask = catalog.Serializer.SerializeCatalogAsync(
                            compressed: compressed,
                            configureAwait: false,
                            cancellationToken: cancellationToken
                            );

                        serializeTasks.Value.Add(serializeTask);
                    }
                }

                var serializedCatalogs = await ValueTaskEx.WhenAll(serializeTasks.Value);

                return new SaveArchiveSerialized(Archive.Path, serializedCatalogs);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return new SaveArchiveSerialized(Archive.Path);
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
