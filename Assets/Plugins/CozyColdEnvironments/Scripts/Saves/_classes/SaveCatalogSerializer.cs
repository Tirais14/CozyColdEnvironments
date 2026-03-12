using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
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
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeGroupsToFileAsync)
                );

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, parameters))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.SerializeGroupsToFileAsyncCore(
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

        public async ValueTask<SaveCatalogSerialized> SerializeCatalogAsync(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeCatalogAsync)
                );

            var result = new ValueReference<SaveCatalogSerialized>();

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, compressed, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.SerializeCatalogAsyncCore(
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

        private async ValueTask SerializeGroupsToFileAsyncCore(
            SerializeToFileParameters parameters = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            using var tasks = ListPool<ValueTask>.Shared.Get();

            ValueTask task;

            try
            {
                lock (Catalog.Groups.SyncRoot)
                {
                    foreach (var (_, group) in Catalog.Groups)
                    {
                        task = group.Serializer.SerializeDataToFileAsync(
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

        private async ValueTask<SaveCatalogSerialized> SerializeCatalogAsyncCore(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            using var serializeTasks = ListPool<ValueTask<SaveGroupSerialized>>.Shared.Get();

            ValueTask<SaveGroupSerialized> serializeTask;

            try
            {
                foreach (var (_, group) in Catalog.Groups)
                {
                    serializeTask = group.Serializer.SerializeGroupAsync(
                        compressed: compressed,
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
        }
    }
}
