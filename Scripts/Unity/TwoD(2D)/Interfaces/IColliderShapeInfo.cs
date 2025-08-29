#nullable enable

namespace CozyColdEnvironments.TwoD
{
    public interface IColliderShapeInfo
    {
        float NearEdgeDistance { get; }
        float FarEdgeDistance { get; }
        float Range { get; }
    }
}