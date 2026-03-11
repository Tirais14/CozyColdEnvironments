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

        public bool RedirectFromFileToSerailizedStorage { get; }

        public SaveGroupSerializer(
            SaveGroup group,
            bool redirectFromFileToSerializedStorage
            )
        {
            Guard.IsNotNull(group, nameof(group));

            Group = group;
            RedirectFromFileToSerailizedStorage = redirectFromFileToSerializedStorage;
        }

        ~SaveGroupSerializer() => Dispose();

        public async ValueTask SerializeDataToFileAsync(
            SerializeToFileParameters parameters = default,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (RedirectFromFileToSerailizedStorage)
            {
                await SerializeDataToFileRedirectedAsync(
                    compressed: parameters.Compressed,
                    configureAwait: configureAwait,
                    cancellationToken: cancellationToken
                    );
            }

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeDataToFileAsync)
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, parameters, configureAwait))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.SerializeDataToFileAsyncCore(
                        parameters: args.parameters,
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
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif
        }

        public async ValueTask<string> SerializeDataAsync(
            bool compressed = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeDataAsync)
                );

            var result = new ValueReference<string>();

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, compressed, configureAwait, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.SerializeDataAsyncCore(
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
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif

            return result;
        }

        public async ValueTask<SaveGroupSerialized> SerializeGroupAsync(
            bool compressed = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToThreadPool();
#endif

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeGroupAsync)
                );

            var result = new ValueReference<SaveGroupSerialized>();

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, compressed, configureAwait, result))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result = await args.@this.SerializeGroupAsyncCore(
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
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToMainThread(configureAwait);
#endif

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
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToThreadPool();
#endif

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
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            finally
            {
                await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
#endif
        }

        private async ValueTask<string> SerializeDataAsyncCore(
            bool compressed = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToThreadPool();
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
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            finally
            {
                await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
#endif
        }

        private async ValueTask SerializeDataToFileRedirectedAsync(
            bool compressed = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            try
            {
                var serializedGroup = await SerializeGroupAsyncCore(
                    compressed: compressed,
                    configureAwait: false,
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
#if !PLATFORM_WEBGL && UNITASK_PLUGIN
            finally
            {
                await CCEnvs.Threading.Tasks.UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
#endif
        }

        private async ValueTask<SaveGroupSerialized> SerializeGroupAsyncCore(
            bool compressed = true,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            CCDisposable.ThrowIfDisposed(this, disposed);

            try
            {
                string serialized = await SerializeDataAsyncCore(
                    compressed: compressed,
                    configureAwait: configureAwait,
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
