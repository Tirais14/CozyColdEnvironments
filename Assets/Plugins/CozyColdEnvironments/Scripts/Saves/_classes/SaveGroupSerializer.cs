using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CCEnvs.Saves;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs
{
    public sealed class SaveGroupSerializer : IDisposable
    {
        private readonly CommandScheduler commandScheduler = CommandScheduler.CreateDefaultRegistered(nameof(SaveGroupSerializer));

        private ReactiveCommand<Unit>? onSerialized;

        public SaveGroup Group { get; }

        public SaveData Data => Group.SaveData;

        public RedirectionMode Redirection => Group.Redirection;

        public SaveGroupSerializer(
            SaveGroup group
            )
        {
            Guard.IsNotNull(group, nameof(group));

            Group = group;
        }

        ~SaveGroupSerializer() => Dispose();

        public async ValueTask SerializeDataToFileAsync(
            SerializeToFileParameters parameters = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (Redirection == RedirectionMode.FromFileToSerializedStorage)
            {
                await SerializeDataToFileRedirectedAsync(
                    compressed: parameters.Compressed,
                    cancellationToken: cancellationToken
                    );
            }

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeDataToFileAsync)
                );

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, parameters))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.SerializeDataToFileAsyncCore(
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

        public async ValueTask<string> SerializeDataAsync(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeDataAsync)
                );

            var result = new ValueReference<string>();

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, compressed, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.SerializeDataAsyncCore(
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

        public async ValueTask<SaveGroupSerialized> SerializeGroupAsync(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeGroupAsync)
                );

            var result = new ValueReference<SaveGroupSerialized>();

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, compressed, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.SerializeGroupAsyncCore(
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
            onSerialized?.Dispose();
        }

        public Observable<Unit> ObserveSerialize()
        {
            onSerialized ??= new ReactiveCommand<Unit>();

            return onSerialized;
        }

        private async ValueTask SerializeDataToFileAsyncCore(
            SerializeToFileParameters parameters = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            try
            {
                string serialized = JsonConvert.SerializeObject(Data, SaveSystem.SerializerSettings);

                var path = Group.GetFullPath();

                await SaveWrite.ToFileAsync(
                    fileContent: serialized,
                    parameters,
                    configureAwait: false,
                    cancellationToken: cancellationToken
                    );
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                throw;
            }
        }

        private async ValueTask<string> SerializeDataAsyncCore(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

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
        }

        private async ValueTask SerializeDataToFileRedirectedAsync(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            try
            {
                var serializedGroup = await SerializeGroupAsyncCore(
                    compressed: compressed,
                    cancellationToken: cancellationToken
                    );

                var archivePath = Group.Catalog.Archive.Path;
                var catalogPath = Group.Catalog.Path;

                SaveSystemSerializedStorage.AddGroup(
                    archivePath,
                    catalogPath,
                    serializedGroup
                    );
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return;
            }
        }

        private async ValueTask<SaveGroupSerialized> SerializeGroupAsyncCore(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            try
            {
                string serialized = await SerializeDataAsyncCore(
                    compressed: compressed,
                    cancellationToken: cancellationToken
                    );

                return new SaveGroupSerialized(Group.Name, serialized);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return new SaveGroupSerialized(Group.Name, string.Empty);
            }
        }
    }
}
