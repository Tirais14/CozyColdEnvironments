#if CC_DEBUG_ENABLED
using CCEnvs.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Diagnostics
{
    [UpdateInGroup(typeof(DiagnosticsSystemGroup))]
    public readonly partial struct LogSystem : ISystem
    {
        private readonly struct MessagesContext { }
        private readonly struct IsDestroyedContext { }

        private struct Message
        {
            public FixedString4096Bytes Value;
            public FixedString4096Bytes Context;
            public LogType LogType;
        }

        public static int MessageCapacity {
            get => messages.Data.IsCreated ? messages.Data.Capacity : 0;
            set
            {
                if (!messages.Data.IsCreated)
                    return;

                messages.Data.Capacity = value;
            }
        }

        private static readonly SharedStatic<NativeList<Message>> messages = SharedStatic<NativeList<Message>>.GetOrCreate<MessagesContext>();

        private static readonly SharedStatic<bool> isDestroyed = SharedStatic<bool>.GetOrCreate<IsDestroyedContext>();

        [BurstCompile]
        public readonly void OnCreate(ref SystemState state)
        {
            messages.Data = new NativeList<Message>();
        }

        public readonly void OnUpdate(ref SystemState state)
        {
            if (!CCDebug.Instance.IsEnabled
                ||
                messages.Data.IsEmpty)
            {
                return;
            }

            foreach (var message in messages.Data)
                CCDebug.PrintDebug(message.Value, message.LogType, message.Context);

            messages.Data.Clear();

            if (messages.Data.Capacity > 16)
                messages.Data.Capacity = 16;
        }

        [BurstCompile]
        public readonly void OnDestroy(ref SystemState state)
        {
            messages.Data.Dispose();
            isDestroyed.Data = true;
        }

        public static void PrintDebug(
            FixedString4096Bytes message,
            LogType logType = LogType.Log,
            FixedString4096Bytes context = default
            )
        {
            if (isDestroyed.Data)
                return;

            if (!messages.Data.IsCreated)
                messages.Data = new NativeList<Message>();

            messages.Data.Add(new Message
            { 
                Value = message,
                Context = context,
                LogType = logType
            });
        }

        public static void PrintLog(
            FixedString4096Bytes message,
            FixedString4096Bytes context = default
            )
        {
            PrintDebug(message, LogType.Log, context);
        }

        public static void PrintWarning(
            FixedString4096Bytes message,
            FixedString4096Bytes context = default
            )
        {
            PrintDebug(message, LogType.Warning, context);
        }

        public static void PrintError(
            FixedString4096Bytes message,
            FixedString4096Bytes context = default
            )
        {
            PrintDebug(message, LogType.Error, context);
        }

        public static void PrintDebugParallel(
            FixedString4096Bytes message,
            LogType logType = LogType.Log,
            FixedString4096Bytes context = default
            )
        {
            if (!messages.Data.IsCreated 
                ||
                isDestroyed.Data
                ||
                messages.Data.Length + 1 >= messages.Data.Capacity)
            {
                return;
            }

            messages.Data.AsParallelWriter().AddNoResize(new Message
            {
                Value = message,
                Context = context,
                LogType = logType
            });
        }

        public static void PrintLogParallel(
            FixedString4096Bytes message,
            FixedString4096Bytes context = default
            )
        {
            PrintDebugParallel(message, LogType.Log, context);
        }

        public static void PrintWarningParallel(
            FixedString4096Bytes message,
            FixedString4096Bytes context = default
            )
        {
            PrintDebugParallel(message, LogType.Warning, context);
        }

        public static void PrintErrorParallel(
            FixedString4096Bytes message,
            FixedString4096Bytes context = default
            )
        {
            PrintDebugParallel(message, LogType.Error, context);
        }
    }
}
#endif
