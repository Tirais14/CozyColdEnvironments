#nullable enable
#pragma warning disable S3881
namespace UTIRLib.Tickables
{
    public class FixedTicker : ATIcker<IFixedTickable>, IFixedTicker
    {
        private void FixedUpdate()
        {
            for (int i = 0; i < tickablesCount; i++)
                tickables[i].DoFixedTick();
        }
    }
}
