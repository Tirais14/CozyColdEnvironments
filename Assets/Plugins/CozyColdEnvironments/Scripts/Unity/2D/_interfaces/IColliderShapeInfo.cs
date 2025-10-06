#nullable enable

namespace CCEnvs.Unity.TwoD
{
    public interface IColliderShapeInfo
    {
        float NearEdgeDistance { get; }
        float FarEdgeDistance { get; }
        float Range { get; }
    }
}