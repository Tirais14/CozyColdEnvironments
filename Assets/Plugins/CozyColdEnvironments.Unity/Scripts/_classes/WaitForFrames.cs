#nullable enable
namespace CCEnvs.UnityX
{
    public sealed class WaitForFrames
    {
        internal int frameCount;

        public WaitForFrames(int frameCount)
        {
            this.frameCount = frameCount;
        }
    }
}
