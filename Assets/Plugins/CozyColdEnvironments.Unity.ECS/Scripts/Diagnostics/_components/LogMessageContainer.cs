using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Diagnostics
{
    public struct LogMessageContainer : IComponentData
    {
        public FixedString512Bytes Message;

        public CCEnvs.Diagnostics.LogType LogType;

        public ComponentType CallerType;
    }
}
