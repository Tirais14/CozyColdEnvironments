using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public static class mathHelper
    {
        public static int3 floorDiv(in int3 value, in int3 divider)
        {
            return new int3(
                MathHelper.FloorDiv(value.x, divider.x),
                MathHelper.FloorDiv(value.y, divider.y),
                MathHelper.FloorDiv(value.z, divider.z)
                );
        }
    }
}
