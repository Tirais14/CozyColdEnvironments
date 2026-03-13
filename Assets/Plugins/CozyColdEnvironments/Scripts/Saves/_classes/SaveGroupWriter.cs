using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Threading;

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

        public void CaptureAndWriteSaveData(
            CaptureAndWriteParameters prms = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(CaptureAndWriteSaveData)
                );

            Command.Builder.WithName(cmdName)
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
                .ScheduleBy(Group.commandScheduler);
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
