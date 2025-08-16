#nullable enable
#pragma warning disable S3881
namespace UTIRLib.Tickables
{
    public class LateTicker : ATIcker<ILateTickable>, ILateTicker
    {
        private void LateUpdate()
        {
            for (int i = 0; i < tickablesCount; i++)
                tickables[i].DoLateTick();
        }
    }
}
