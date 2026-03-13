using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Saves
{
    public sealed class SaveArchiveWriter : IDisposable
    {
        private ReactiveCommand<WriteEventInfo<SaveArchive>>? onWritten;

        public SaveArchive Archive { get; }

        public SaveArchiveWriter(SaveArchive archive)
        {
            Guard.IsNotNull(archive, nameof(archive));

            Archive = archive;
        }

        ~SaveArchiveWriter() => Dispose();

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
                .ScheduleBy(Archive.commandScheduler)
                .WaitForDone();
        }

        public Observable<WriteEventInfo<SaveArchive>> ObserveWrite()
        {
            onWritten ??= new ReactiveCommand<WriteEventInfo<SaveArchive>>();
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

            PooledArray<SaveCatalog> catalogsCopy;

            lock (Archive.Catalogs.SyncRoot)
                catalogsCopy = Archive.Catalogs.SelectValue().EnumerableToArrayPooled(Archive.Catalogs.Count);

            try
            {
                foreach (var catalog in catalogsCopy)
                {
                    await catalog.Writer.CaptureAndWriteGroupsAsync(
                        prms: prms,
                        cancellationToken: cancellationToken
                        );
                }

                if (onWritten is not null)
                {
                    var evInfo = new WriteEventInfo<SaveArchive>(Archive, prms.WriteMode);
                    onWritten.Execute(evInfo);
                }
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return;
            }
            finally
            {
                catalogsCopy.Dispose();
            }
        }
    }
}
