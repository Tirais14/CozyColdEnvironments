#nullable enable
#pragma warning disable S3881
namespace UTIRLib.Tickables
{
    public class Ticker : ATIcker<ITickable>, ITicker
    {
        private void Update()
        {
            for (int i = 0; i < tickablesCount; i++)
                tickables[i].DoTick();
        }
    }
}
