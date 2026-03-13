using CCEnvs.Collections;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;
using ValueTaskSupplement;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveSystemLoader
    {
        private readonly static Type selfType = typeof(SaveSystemLoader);

        private static ReactiveCommand<Unit>? onLoaded;

        public static async ValueTask LoadArchivesFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (SaveSystem.Archives.IsEmpty())
                return;

            string cmdName = NameFactory.CreateFromCaller(
                selfType,
                nameof(LoadArchivesFromFileAsync),
                expirationTimeRelativeToNow: TimeSpan.Zero
                );

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState(writeSaveDataMode)
                .Asynchronously()
                .WithExecuteAction(
                static async (writeSaveDataMode, cancellationToken) =>
                {
                    await LoadArchivesFromFileAsyncCore(
                        writeSaveDataMode: writeSaveDataMode,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(SaveSystem.CommandScheduler)
                .WaitForDone();
        }

        public static async ValueTask LoadArchivesFromSerializedAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (SaveSystem.Archives.IsEmpty())
                return;

            if (SaveSystemSerializedStorage.Archives.IsEmpty())
                return;

            string cmdName = NameFactory.CreateFromCaller(
                selfType,
                nameof(LoadArchivesFromSerializedAsync),
                expirationTimeRelativeToNow: TimeSpan.Zero
                );

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState(writeSaveDataMode)
                .Asynchronously()
                .WithExecuteAction(
                static async (writeSaveDataMode, cancellationToken) =>
                {
                    await LoadArchivesFromSerializedAsyncCore(
                        writeSaveDataMode: writeSaveDataMode,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(SaveSystem.CommandScheduler)
                .WaitForDone();
        }

        public static Observable<Unit> ObserveLoad()
        {
            onLoaded ??= new ReactiveCommand<Unit>();
            return onLoaded;
        }

        private static async ValueTask LoadArchivesFromFileAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var tasks = ListPool<ValueTask>.Shared.Get();

#pragma warning disable CS4014
            tasks.Value.TryIncreaseCapacity(SaveSystem.Archives.Count);
#pragma warning restore CS4014

            ValueTask task;

            try
            {
                lock (SaveSystem.Archives.SyncRoot)
                {
                    foreach (var (_, archive) in SaveSystem.Archives)
                    {
                        task = archive.Loader.LoadCatalogsFromFileAsync(
                            writeSaveDataMode,
                            cancellationToken: cancellationToken
                            );

                        tasks.Value.Add(task);
                    }
                }

                await ValueTaskEx.WhenAll(tasks.Value);

                onLoaded?.Execute(Unit.Default);
            }
            catch (Exception ex)
            {
                selfType.PrintException(ex);
                return;
            }
        }

        private static async ValueTask LoadArchivesFromSerializedAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var tasks = ListPool<ValueTask>.Shared.Get();

#pragma warning disable CS4014
            tasks.Value.TryIncreaseCapacity(SaveSystem.Archives.Count);
#pragma warning restore CS4014

            ValueTask task;

            try
            {
                lock (SaveSystemSerializedStorage.ArchivesGate)
                {
                    foreach (var (_, serializedArchive) in SaveSystemSerializedStorage.Archives)
                    {
                        if (!SaveSystem.Archives.TryGetValue(serializedArchive.Path, out var archive))
                            continue;

                        task = archive.Loader.LoadCatalogsFromSerializedAsync(
                            serialized: serializedArchive,
                            writeSaveDataMode: writeSaveDataMode,
                            cancellationToken: cancellationToken
                            );

                        tasks.Value.Add(task);
                    }
                }

                await ValueTaskEx.WhenAll(tasks.Value);

                onLoaded?.Execute(Unit.Default);
            }
            catch (Exception ex)
            {
                selfType.PrintException(ex);
                return;
            }
        }
    }
}
