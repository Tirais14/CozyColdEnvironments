using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using ValueTaskSupplement;

#nullable enable
namespace CCEnvs.Saves
{
    public sealed class SaveCatalogSerializer
    {
        public SaveCatalog Catalog { get; }

        public SaveCatalogSerializer(SaveCatalog catalog)
        {
            Guard.IsNotNull(catalog, nameof(catalog));

            Catalog = catalog;
        }

        public async ValueTask SerializeGroupsToFileAsync(
            SerializeToFileParameters parameters = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

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
                .ScheduleBy(Catalog.commandScheduler)
                .WaitForDone();
        }

        public async ValueTask<SaveCatalogSerialized> SerializeCatalogAsync(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeCatalogAsync)
                );

            using var result = ValueReferencePool<SaveCatalogSerialized>.Shared.Get();

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, compressed, result: result.Value))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result.Value = await args.@this.SerializeCatalogAsyncCore(
                        compressed: args.compressed,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(Catalog.commandScheduler)
                .WaitForDone();

            return result.Value;
        }

        private async ValueTask SerializeGroupsToFileAsyncCore(
            SerializeToFileParameters parameters = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

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
