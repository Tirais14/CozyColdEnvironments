#nullable enable
using System;

namespace CCEnvs.Patterns.States
{
    public interface IState
    {
        string ID { get; }

        void Enter() { }

        void Tick() { }

        void FixedTick() { }

        void LateTick() { }

        void Exit() { }
    }
}
