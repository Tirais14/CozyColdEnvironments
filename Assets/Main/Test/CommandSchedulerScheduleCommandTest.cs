using System;
using System.Collections.Generic;
using CCEnvs;
using CCEnvs.Patterns.Commands;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using R3;
using UnityEditor;
using UnityEngine;

#nullable enable
namespace Tests
{
    public class CommandSchedulerScheduleCommandTest : CCBehaviour
    {
        private const int CMD_COUNT = 256;

        private readonly CommandScheduler commandScheduler = new(UnityFrameProvider.Update);

        private readonly List<ICommandBase> commands = new(CMD_COUNT);

        public void Launch()
        {
            for (int i = 0; i < CMD_COUNT; i++)
                commands.Add(CreateCommand());

            foreach (var cmd in commands)
                cmd.ScheduleBy(commandScheduler);

            commands.Clear();
        }

        private ICommandBase CreateCommand()
        {
            var cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(CreateCommand)
                );

            return Command.Builder.WithName(cmdName)
                .WithState(this)
                .Asynchronously()
                .WithExecuteAction(
                static async (@this, cancellationToken) =>
                {
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken);
        }
    }
}
