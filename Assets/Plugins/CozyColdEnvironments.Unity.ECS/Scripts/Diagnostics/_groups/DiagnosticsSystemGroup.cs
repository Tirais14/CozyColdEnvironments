using Unity.Entities;

namespace CCEnvs.UnityX.ECS.Diagnostics
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial class DiagnosticsSystemGroup : ComponentSystemGroup
    {
    }
}
