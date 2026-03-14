#nullable enable
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using System;
using System.Threading;
using System.Threading.Tasks;
using ValueTaskSupplement;

namespace CCEnvs.Saves
{
    public static partial class SaveSystem
    {
        public static class Serializer
        {
            private readonly static Type selfType = typeof(Serializer);

            public static async ValueTask SerializeArchivesToFileAsync(
                SerializeToFileParameters parameters = default,
                CancellationToken cancellationToken = default
                )
            {
                cancellationToken.ThrowIfCancellationRequested();

                string cmdName = NameFactory.CreateFromCaller(
                    selfType,
                    nameof(SerializeArchivesToFileAsync)
                    );

                await Command.Builder.WithName(cmdName)
                    .OnThreadPool()
                    .WithState(parameters)
                    .Asynchronously()
                    .WithExecuteAction(
                    static async (parameters, cancellationToken) =>
                    {
                        await SerializeArchivesToFileAsyncCore(
                            parameters: parameters,
                            cancellationToken: cancellationToken
                            );
                    })
                    .BuildPooled()
                    .Value
                    .AttachExternalCancellationToken(cancellationToken)
                    .ScheduleBy(CommandScheduler)
                    .WaitForDone();
            }

            public static async ValueTask<SaveArchiveSerialized[]> SerializeArchivesAsync(
                bool compressed = true,
                CancellationToken cancellationToken = default
                )
            {
                cancellationToken.ThrowIfCancellationRequested();

                string cmdName = NameFactory.CreateFromCaller(
                    selfType,
                    nameof(SerializeArchivesAsync)
                    );

                using var result = ValueReferencePool<SaveArchiveSerialized[]>.Shared.Get();

                await Command.Builder.WithName(cmdName)
                    .OnThreadPool()
                    .WithState((compressed, result: result.Value))
                    .Asynchronously()
                    .WithExecuteAction(
                    static async (args, cancellationToken) =>
                    {
                        args.result.Value = await SerializeArchivesAsyncCore(
                            compressed: args.compressed,
                            cancellationToken: cancellationToken
                            );
                    })
                    .BuildPooled()
                    .Value
                    .AttachExternalCancellationToken(cancellationToken)
                    .ScheduleBy(CommandScheduler)
                    .WaitForDone();

                return result.Value;
            }

            private static async ValueTask SerializeArchivesToFileAsyncCore(
                SerializeToFileParameters parameters = default,
                CancellationToken cancellationToken = default
                )
            {
                cancellationToken.ThrowIfCancellationRequested();

                var tasks = ListPool<ValueTask>.Shared.Get();

                ValueTask task;

                try
                {
                    lock (Archives.SyncRoot)
                    {
                        foreach (var (_, archive) in Archives)
                        {
                            task = archive.Serializer.SerializeCatalogsToFileAsync(
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
                    selfType.PrintException(ex);
                    tasks.Dispose();

                    return;
                }
            }

            private static async ValueTask<SaveArchiveSerialized[]> SerializeArchivesAsyncCore(
                bool compressed = true,
                CancellationToken cancellationToken = default
                )
            {
                cancellationToken.ThrowIfCancellationRequested();

                var serializeTasks = ListPool<ValueTask<SaveArchiveSerialized>>.Shared.Get();

                ValueTask<SaveArchiveSerialized> serializeTask;

                try
                {
                    lock (Archives.SyncRoot)
                    {
                        foreach (var (_, archive) in Archives)
                        {
                            serializeTask = archive.Serializer.SerializeArchiveAsync(
                                compressed: compressed,
                                cancellationToken: cancellationToken
                                );

                            serializeTasks.Value.Add(serializeTask);
                        }
                    }

                    return await ValueTaskEx.WhenAll(serializeTasks.Value);
                }
                catch (Exception ex)
                {
                    selfType.PrintException(ex);
                    serializeTasks.Dispose();

                    return Array.Empty<SaveArchiveSerialized>();
                }
            }
        }
    }
}
