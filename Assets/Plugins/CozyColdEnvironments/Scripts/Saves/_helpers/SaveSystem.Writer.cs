#nullable enable
using CCEnvs.Collections;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Saves
{
    public static partial class SaveSystem
    {   
        public static class Writer
        {
            private readonly static object onWrittenTargetMock = new();

            private readonly static Type selfType = typeof(Writer);

            private static ReactiveCommand<WriteEventInfo<object>>? onWritten;

            public static async ValueTask CaptureAndWriteArchivesAsync(
                CaptureAndWriteParameters prms = default,
                CancellationToken cancellationToken = default
                )
            {
                cancellationToken.ThrowIfCancellationRequested();

                string cmdName = NameFactory.CreateFromCaller(
                    selfType,
                    nameof(CaptureAndWriteArchivesAsync)
                    );

                await Command.Builder.WithName(cmdName)
                    .WithState(prms)
                    .Asynchronously()
                    .WithExecuteAction(
                    static async (prms, cancellationToken) =>
                    {
                        await CaptureAndWriteArchivesAsyncCore(
                            prms: prms,
                            cancellationToken: cancellationToken
                            );
                    })
                    .BuildPooled()
                    .Value
                    .AttachExternalCancellationToken(cancellationToken)
                    .ScheduleBy(CommandScheduler)
                    .WaitForDone();
            }

            public static Observable<WriteEventInfo<object>> ObserveWrite()
            {
                onWritten ??= new ReactiveCommand<WriteEventInfo<object>>();
                return onWritten;
            }

            private static async ValueTask CaptureAndWriteArchivesAsyncCore(
                CaptureAndWriteParameters prms = default,
                CancellationToken cancellationToken = default
                )
            {
                cancellationToken.ThrowIfCancellationRequested();

                PooledArray<SaveArchive> archivesCopy;

                lock (Archives.SyncRoot)
                    archivesCopy = Archives.SelectValue().EnumerableToArrayPooled(SaveSystem.Archives.Count);

                try
                {
                    foreach (var archive in archivesCopy)
                    {
                        await archive.Writer.CaptureAndWriteGroupsAsync(
                            prms: prms,
                            cancellationToken: cancellationToken
                            );
                    }

                    if (onWritten is not null)
                    {
                        var evInfo = new WriteEventInfo<object>(onWrittenTargetMock, prms.WriteMode);
                        onWritten.Execute(evInfo);
                    }
                }
                catch (Exception ex)
                {
                    selfType.PrintException(ex);
                    return;
                }
                finally
                {
                    archivesCopy.Dispose();
                }
            }
        }
    }
}
