#nullable enable
using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public sealed class CompositeCommand : CommandAsync
    {
        private readonly Queue<ICommandAsync> commands = new();
        private readonly Func<bool>? isReadyToExecute;
        private readonly ICommandScheduler commandScheduler;
        private readonly CompositeDisposable disposables = new();

        public override bool IsReadyToExecute {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => base.IsReadyToExecute && (isReadyToExecute?.Invoke() ?? true);
        }

        public CompositeCommand(
            ICommandScheduler commandScheduler,
            Func<bool>? isReadyToExecute = null,
            bool isSingle = false,
            string? name = null,
            bool isResetable = true,
            int delayFrameCount = 0
            )
            :
            base(isSingle: isSingle,
                name: name,
                isResetable: isResetable,
                delayFrameCount: delayFrameCount)
        {
            this.commandScheduler = commandScheduler;
            this.isReadyToExecute = isReadyToExecute;
        }

        public CompositeCommand Add(ICommandAsync cmd, CommandStatus continuationStatus = CommandStatus.Completed)
        {
            CC.Guard.IsNotNull(cmd, nameof(cmd));

            Guard.IsTrue(
                continuationStatus != CommandStatus.None,
                nameof(continuationStatus),
                $"Invalid status: {continuationStatus}"
                );

            if (cmd.IsDone)
                return this;

            SubscribeCommand(cmd, continuationStatus);

            commands.Enqueue(cmd);

            return this;
        }

        protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
        {
            if (!commands.TryDequeue(out var initialCmd))
                return;

            commandScheduler.Schedule(initialCmd);

#if UNITASK_PLUGIN
            await Cysharp.Threading.Tasks.UniTask.WaitUntil(this,
                static @this =>
                {
                    return @this.commands.IsEmpty();
                });
#else
            while (commands.IsNotEmpty())
                await Task.Yield();
#endif
        }

        private bool disposed;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposed)
                return;

            if (disposing)
            {
                disposables.Dispose();

                foreach (var cmd in commands)
                    cmd.Dispose();
            }

            disposed = true;
        }

        private void SubscribeCommand(ICommandAsync cmd, CommandStatus continuationStatus)
        {
            cmd.ObserveIsDone()
                .Skip(1)
                .Subscribe(new { This = this, Command = cmd, ContinuationStatus = continuationStatus },
                static (doneCmdStatus, args) =>
                {
                    if (doneCmdStatus != args.ContinuationStatus)
                        return;

                    if (!args.This.commands.TryDequeue(out var nextCmdInfo))
                        return;

                    args.This.commandScheduler.Schedule(nextCmdInfo);
                })
                .AddTo(disposables);
        }
    }
}
