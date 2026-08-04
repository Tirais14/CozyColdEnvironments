using CCEnvs.TypeMatching;
using System;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.UnityX
{
    public static class UnityObjectHelper
    {
        public static bool DestroyByGameObject(this Object source)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.Is<GameObject>(out var go))
            {
                Object.Destroy(go);
                return true;
            }
            else if (source.Is<Component>(out var cmp))
            {
                Object.Destroy(cmp.gameObject);
                return true;
            }

            return false;
        }

        public static async
#if UNITASK_PLUGIN
            Cysharp.Threading.Tasks.UniTask
#else
            System.Threading.Tasks.ValueTask
#endif
            <T[]> InstantiateAsync<T>(
            T instance,
            int count = 1,
            InstantiateParameters parameters = default,
            CancellationToken cancellationToken = default
            )
            where T : Object
        {
            cancellationToken.ThrowIfCancellationRequested();

            AsyncInstantiateOperation<T> instantiateOperation = Object.InstantiateAsync(
                instance,
                count,
                parameters,
                cancellationToken
                );

            try
            {
                return await instantiateOperation;
            }
            catch (OperationCanceledException)
            {
                for (int i = 0; i < instantiateOperation.Result.Length; i++)
                    DestroyByGameObject(instantiateOperation.Result[i]);

                throw;
            }
        }
    }
}
