using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public class AnonymousState : IState
    {
        private readonly Action? enter;
        private readonly Action? update;
        private readonly Action? fixedUpdate;
        private readonly Action? lateUpdate;
        private readonly Action? exit;

        public string ID { get; }

        public AnonymousState(
            string id,
            Action? enter = null,
            Action? update = null,
            Action? fixedUpdate = null,
            Action? lateUpdate = null,
            Action? exit = null
            )
        {
            Guard.IsNotNull(id, nameof(id));

            ID = id;
            this.enter = enter;
            this.update = update;
            this.fixedUpdate = fixedUpdate;
            this.lateUpdate = lateUpdate;
            this.exit = exit;
        }

        public void Enter()
        {
            enter?.Invoke();
        }

        public void Update()
        {
            update?.Invoke();
        }

        public void FixedUpdate()
        {
            fixedUpdate?.Invoke();
        }

        public void LateUpdate()
        {
            lateUpdate?.Invoke();
        }

        public void Exit()
        {
            exit?.Invoke();
        }
    }

    public class AnonymousState<TArg> : IState
    {
        private readonly TArg arg;

        private readonly Action<TArg>? enter;
        private readonly Action<TArg>? update;
        private readonly Action<TArg>? fixedUpdate;
        private readonly Action<TArg>? lateUpdate;
        private readonly Action<TArg>? exit;

        public string ID { get; }

        public AnonymousState(
            string id,
            TArg arg,
            Action<TArg>? enter = null,
            Action<TArg>? update = null,
            Action<TArg>? fixedUpdate = null,
            Action<TArg>? lateUpdate = null,
            Action<TArg>? exit = null
            )
        {
            Guard.IsNotNull(id, nameof(id));

            ID = id;
            this.arg = arg;
            this.enter = enter;
            this.update = update;
            this.fixedUpdate = fixedUpdate;
            this.lateUpdate = lateUpdate;
            this.exit = exit;
        }

        public void Enter()
        {
            enter?.Invoke(arg);
        }

        public void Update()
        {
            update?.Invoke(arg);
        }

        public void FixedUpdate()
        {
            fixedUpdate?.Invoke(arg);
        }

        public void LateUpdate()
        {
            lateUpdate?.Invoke(arg);
        }

        public void Exit()
        {
            exit?.Invoke(arg);
        }
    }
}
