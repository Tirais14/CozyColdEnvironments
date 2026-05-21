using Unity.Burst;
using Unity.Entities.Content;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public static class WeakObjectReferenceExtensions
    {
        [BurstCompile]
        public static bool IsLoaded<T>(this in WeakObjectReference<T> objRef)
            where T : UnityEngine.Object
        {
            return objRef.IsReferenceValid 
                   ||
                   objRef.LoadingStatus == ObjectLoadingStatus.Completed;
        }

        [BurstCompile]
        public static bool IsLoading<T>(this in WeakObjectReference<T> objRef)
            where T : UnityEngine.Object
        {
            return objRef.LoadingStatus == ObjectLoadingStatus.Queued
                   ||
                   objRef.LoadingStatus == ObjectLoadingStatus.Loading;
        }

        [BurstCompile]
        public static bool IsLoadingFaulted<T>(this in WeakObjectReference<T> objRef)
            where T : UnityEngine.Object
        {
            return objRef.LoadingStatus == ObjectLoadingStatus.Error;
        }
    }
}
