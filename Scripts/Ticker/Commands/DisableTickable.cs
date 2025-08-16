using UnityEngine;
using UTIRLib.Patterns.Commands;

#nullable enable
namespace UTIRLib.Tickables
{
    public class DisableTickable : ATickerCommand<ITickableBase>
    {
        public DisableTickable(ITickableBase tickable)
            :
            base(tickable)
        {
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
