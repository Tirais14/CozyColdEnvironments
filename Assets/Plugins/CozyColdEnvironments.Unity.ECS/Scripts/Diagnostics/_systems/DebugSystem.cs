#if CC_DEBUG_ENABLED
using CCEnvs.Diagnostics;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Diagnostics
{
    public partial struct DebugSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!CCDebug.Instance.IsEnabled)
                return;

            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (messageContainerRef, entity) in SystemAPI.Query<RefRO<DebugMessageContainer>>().WithEntityAccess())
            {
                var messageContainer = messageContainerRef.ValueRO;

                if (messageContainer.CallerType != default)
                {
                    var callerManagedType = messageContainer.CallerType.GetManagedType();

                    if (!CCDebug.IsTypeEnabled(callerManagedType))
                        continue;

                    callerManagedType.PrintDebug(messageContainer.Message.ConvertToString(), messageContainer.LogType);
                }
                else
                    CCDebug.Instance.PrintDebug(messageContainer.Message.ConvertToString(), messageContainer.LogType);

                ecb.RemoveComponent<DebugMessageContainer>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
#endif
