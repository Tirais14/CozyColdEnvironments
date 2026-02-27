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
        private readonly CommandScheduler commandScheduler = new(UnityFrameProvider.Update);

        private readonly List<ICommandBase> commands = new(16384);

        public void Launch()
        {
            for (int i = 0; i < 16384; i++)
                commands.Add(CreateCommand());

            UniTask.Create(this,
                static async @this =>
                {
                    await UniTask.WaitForSeconds(3f);

                    foreach (var cmd in @this.commands)
                        cmd.ScheduleBy(@this.commandScheduler);

#if UNITY_EDITOR
                    EditorApplication.isPaused = true;
#endif
                })
                .ForgetByPrintException();
        }

        private ICommandBase CreateCommand()
        {
            var cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(CreateCommand)
                );

            return Command.Builder.SetName(cmdName)
                .WithState(this)
                .Asyncronously()
                .SetExecuteAction(
                static async (@this, cancellationToken) =>
                {
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken);
        }
    }
}
