using System;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public class AnonymousState : IState
    {
        private readonly Action enter;
        private readonly Action update;
        private readonly Action fixedUpdate;
        private readonly Action lateUpdate;
        private readonly Action exit;

        public string ID { get; }

        public AnonymousState(string id)
        {

        }

        public void Enter()
        {

        }

        public void Update()
        {

        }

        public void FixedUpdate()
        {

        }

        public void LateUpdate()
        {

        }

        public void Exit()
        {

        }
    }
}
