using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Saves
{
    public class SaveCatalogWriter : IDisposable
    {
        private ReactiveCommand<WriteEventInfo<SaveCatalog>>? onWritten;

        public SaveCatalog Catalog { get; }

        public SaveCatalogWriter(SaveCatalog catalog)
        {
            Guard.IsNotNull(catalog, nameof(catalog));

            Catalog = catalog;
        }

        public async ValueTask CaptureAndWriteGroupsAsync(
            CaptureAndWriteParameters prms = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(CaptureAndWriteGroupsAsyncCore)
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, prms))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.CaptureAndWriteGroupsAsyncCore(
                        prms: args.prms,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(Catalog.commandScheduler)
                .WaitForDone();
        }

        public Observable<WriteEventInfo<SaveCatalog>> ObserveWrite()
        {
            onWritten ??= new ReactiveCommand<WriteEventInfo<SaveCatalog>>();
            return onWritten;
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            onWritten?.Dispose();

            GC.SuppressFinalize(this);
        }

        private async ValueTask CaptureAndWriteGroupsAsyncCore(
            CaptureAndWriteParameters prms = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            using var toProcessGroups = QueuePool<SaveGroup>.Shared.Get();

            try
            {
                await CaptureAndWriteBasicGroups(prms, cancellationToken);
                await CaptureAndWriteIncrementalGroups(prms, cancellationToken);

                if (onWritten is not null)
                {
                    var evInfo = new WriteEventInfo<SaveCatalog>(Catalog, prms.WriteMode);
                    onWritten.Execute(evInfo);
                }
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return;
            }
        }

        private async ValueTask CaptureAndWriteBasicGroups(
            CaptureAndWriteParameters prms = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            PooledArray<SaveGroup> groupsCopy;

            lock (Catalog.Groups.SyncRoot)
                groupsCopy = Catalog.Groups.SelectValue().EnumerableToArrayPooled(Catalog.Groups.Count);

            try
            {
                int enqueudObjectSum = 0;

                SaveGroup group;

                for (int i = 0; i < groupsCopy.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    group = groupsCopy[i];

                    enqueudObjectSum += group.ObservableObjects.Count;

                    try
                    {
                        group.Writer.CaptureAndWriteSaveDataAsync(prms);
                    }
                    catch (Exception ex)
                    {
                        this.PrintException(ex);
                    }

                    if (prms.PreferedObjectLimitPerFrame is not null
                        &&
                        enqueudObjectSum >= prms.PreferedObjectLimitPerFrame)
                    {
                        await UniTask.NextFrame(cancellationToken: cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return;
            }
            finally
            {
                groupsCopy.Dispose();
            }
        }

        private async ValueTask CaptureAndWriteIncrementalGroups(
            CaptureAndWriteParameters prms = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            PooledArray<SaveGroupIncremental> groupsIncrementalCopy;

            lock (Catalog.Groups.SyncRoot)
                groupsIncrementalCopy = Catalog.IncrementalGroups.SelectValue().EnumerableToArrayPooled(Catalog.Groups.Count);

            int enqueudObjectSum = 0;

            SaveGroupIncremental groupIncr;

            for (int i = 0; i < groupsIncrementalCopy.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                groupIncr = groupsIncrementalCopy[i];

                if (!groupIncr.HasDirtyObjects)
                    continue;

                enqueudObjectSum += groupIncr.DirtyObjectCount;

                try
                {
                    groupIncr.Writer.CaptureAndWriteSaveDataAsync(prms);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }

                if (prms.PreferedObjectLimitPerFrame is not null
                    &&
                    enqueudObjectSum >= prms.PreferedObjectLimitPerFrame)
                {
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                }
            }
        }
    }
}
