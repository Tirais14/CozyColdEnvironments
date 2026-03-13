using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Saves;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs
{
    public sealed class SaveGroupSerializer
    {
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

        public async ValueTask SerializeDataToFileAsync(
            SerializeToFileParameters parameters = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

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
                .ScheduleBy(Group.commandScheduler)
                .WaitForDone();
        }

        public async ValueTask<string> SerializeDataAsync(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeDataAsync)
                );

            using var result = ValueReferencePool<string>.Shared.Get();

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, compressed, result: result.Value))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result.Value = await args.@this.SerializeDataAsyncCore(
                        compressed: args.compressed,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(Group.commandScheduler)
                .WaitForDone();

            return result.Value;
        }

        public async ValueTask<SaveGroupSerialized> SerializeGroupAsync(
            bool compressed = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(SerializeGroupAsync)
                );

            using var result = ValueReferencePool<SaveGroupSerialized>.Shared.Get();

            await Command.Builder.WithName(cmdName)
                .OnThreadPool()
                .WithState((@this: this, compressed, result: result.Value))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    args.result.Value = await args.@this.SerializeGroupAsyncCore(
                        compressed: args.compressed,
                        cancellationToken: cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(Group.commandScheduler)
                .WaitForDone();

            return result.Value;
        }

        private async ValueTask SerializeDataToFileAsyncCore(
            SerializeToFileParameters parameters = default,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

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
