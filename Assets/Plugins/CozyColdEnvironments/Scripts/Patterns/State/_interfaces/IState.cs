#nullable enable
using System;

namespace CCEnvs.Patterns.States
{
    public interface IState
    {
        void Enter()
        {
        }

        void Tick()
        {
        }

        void FixedTick()
        { 
        }

        void LateTick()
        {
        }

        void Exit() 
        {
        }
    }
}
