using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.Unity.ECS.Diagnostics
{
    public struct DebugMessageContainer : IComponentData
    {
        public FixedString512Bytes Message;

        public CCEnvs.Diagnostics.LogType LogType;

        public ComponentType CallerType;
    }
}
