using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CCEnvs.Saves;
using CCEnvs.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs
{
    public sealed class SaveGroupDataWriter : IDisposable
    {
        private readonly CommandScheduler commandScheduler = CommandScheduler.CreateDefaultRegistered(nameof(SaveGroupDataWriter));

        private ReactiveCommand<SaveData>? onWritten;

        public SaveGroup Group { get; }

        public SaveData Data => Group.SaveData;

        public SaveGroupDataWriter(SaveGroup group)
        {
            Guard.IsNotNull(group, nameof(group));

            Group = group;
        }

        ~SaveGroupDataWriter() => Dispose();

        public async ValueTask WriteToFileAsync(
            string fileExtension = SaveWrite.DEFAULT_SAVE_EXTENSION,
            bool compressed = true,
            bool backuped = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(WriteToFileAsync)
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, fileExtension, compressed, backuped, configureAwait))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.WriteToFileAsyncCore(
                        fileExtension: args.fileExtension,
                        compressed: args.compressed,
                        backuped: args.backuped,
                        configureAwait: args.configureAwait,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(commandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
        }

        public async ValueTask<string> WriteToTextAsync(
            bool compressed = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(WriteToTextAsync)
                );

            var result = new ValueReference<string>();

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, compressed, configureAwait, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.WriteToTextAsyncCOre(
                        compressed: args.compressed,
                        configureAwait: args.configureAwait,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(commandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif

            return result;
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            commandScheduler.Dispose();
            onWritten?.Dispose();
        }

        public Observable<SaveData> ObserveWrite()
        {
            onWritten ??= new ReactiveCommand<SaveData>();

            return onWritten;
        }

        private async ValueTask WriteToFileAsyncCore(
            string fileExtension = SaveWrite.DEFAULT_SAVE_EXTENSION,
            bool compressed = true,
            bool backuped = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            cancellationToken.ThrowIfCancellationRequested();

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            string serializedEntries = JsonConvert.SerializeObject(Data);

            var path = Group.GetFullPath();

            var parameters = new WriteSaveDataToFileParameters(
                path,
                fileExtension
                )
            {
                Compressed = compressed,
                Backuped = backuped,
                FileContent = serializedEntries
            };

            await SaveWrite.ToFileAsync(
                parameters,
                configureAwait: false,
                cancellationToken: cancellationToken
                );

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
        }

        private async ValueTask<string> WriteToTextAsyncCOre(
            bool compressed = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await UniTaskHelper.TrySwitchToThreadPool();
#endif

            try
            {
                return await SaveSerializer.SerializeAsync(
                    Data,
                    compressed: compressed,
                    cancellationToken: cancellationToken
                    );
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return string.Empty;
            }
            finally
            {
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
            }
        }
    }
}
