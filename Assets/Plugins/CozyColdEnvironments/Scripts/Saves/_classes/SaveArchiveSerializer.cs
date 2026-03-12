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

        public SaveArchiveSerializer(SaveArchive archive)
        {
            Guard.IsNotNull(archive, nameof(archive));

            Archive = archive;
        }

        ~SaveArchiveSerializer() => Dispose();

        public async ValueTask SerializeCatalogsToFileAsync(
            SerializeToFileParameters parameters = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);


            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeCatalogsToFileAsync)
                );

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, parameters))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.SerializeCatalogsToFileAsyncCore(
                        parameters: args.parameters,
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

        public async ValueTask<SaveArchiveSerialized> SerializeArchiveAsync(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeArchiveAsync)
                );

            var result = new ValueReference<SaveArchiveSerialized>();

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, compressed, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.SerializeArchiveAsyncCore(
                        compressed: args.compressed,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(commandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

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
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

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
        }

        private async ValueTask<SaveArchiveSerialized> SerializeArchiveAsyncCore(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

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
        }
    }
}
