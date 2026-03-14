using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Saves
{
    public class SaveGroupWriter : IDisposable
    {
        private ReactiveCommand<WriteEventInfo<SaveGroup>>? onWritten;

        public SaveGroup Group { get; }

        public SaveGroupWriter(SaveGroup group)
        {
            Guard.IsNotNull(group, nameof(group));

            Group = group;
        }

        ~SaveGroupWriter() => Dispose();

        public async ValueTask CaptureAndWriteSaveDataAsync(
            CaptureAndWriteParameters prms = default,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(CaptureAndWriteSaveDataAsync)
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, prms))
                .Synchronously()
                .WithExecuteAction(
                static args =>
                {
                    args.@this.CaptureAndWriteSaveDataCore(
                        prms: args.prms
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(Group.commandScheduler)
                .WaitForDone();
        }

        public Observable<WriteEventInfo<SaveGroup>> ObserveWrite()
        {
            onWritten ??= new ReactiveCommand<WriteEventInfo<SaveGroup>>();
            return onWritten;
        }

        private int disposed;
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            onWritten?.Dispose();

            GC.SuppressFinalize(this);
        }

        private void CaptureAndWriteSaveDataCore(
            CaptureAndWriteParameters prms = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            using var saveUnits = Group.CreateAndProcessSaveEntriesPooled();

            Group.SaveData.Write(saveUnits.Value, prms.WriteMode);

            if (onWritten is not null)
            {
                var evInfo = new WriteEventInfo<SaveGroup>(Group, prms.WriteMode);
                onWritten.Execute(evInfo);
            }
        }
    }
}
