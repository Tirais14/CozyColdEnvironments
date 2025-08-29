#nullable enable
using System;

namespace CozyColdEnvironments.Patterns.States
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
