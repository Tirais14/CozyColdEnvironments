using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public static class SavingSystemExtensions
    {
        /// <inheritdoc cref="ISavingSystem.RegisterObject{TObject}(TObject, string, SceneInfo?)"/>
        public static async UniTask<IDisposable> SavingSystemRegisterObjectAsync<TObject>(
            this TObject source,
            string key,
            SceneInfo sceneInfo = default,
            CancellationToken cancellationToken = default
            )
            where TObject : class
        {
            return await SavingSystem.Self.RegisterObjectAsync(
                obj: source,
                key: key,
                sceneInfo: sceneInfo,
                cancellationToken: cancellationToken
                );
        }

        /// <inheritdoc cref="ISavingSystem.RegisterObject{TObject}(TObject, Func{TObject, string}, SceneInfo?)"/>
        public static async UniTask<IDisposable> SavingSystemRegisterObjectAsync<TObject>(
            this TObject source,
            Func<TObject, string> keySelector,
            SceneInfo sceneInfo = default,
            CancellationToken cancellationToken = default
            )
            where TObject : class
        {
            return await SavingSystem.Self.RegisterObjectAsync(
                obj: source,
                keySelector: keySelector,
                sceneInfo: sceneInfo,
                cancellationToken: cancellationToken
                );
        }

        /// <inheritdoc cref="ISavingSystem.RegisterObject{TObject, TState}(TObject, TState, Func{TObject, TState, string}, SceneInfo?)"/>
        public static async UniTask<IDisposable> SavingSystemRegisterObjectAsync<TObject, TState>(
            this TObject source,
            TState state,
            Func<TObject, TState, string> keySelector,
            SceneInfo sceneInfo = default,
            CancellationToken cancellationToken = default)
            where TObject : class
        {
            return await SavingSystem.Self.RegisterObjectAsync(
                source,
                state,
                keySelector,
                sceneInfo: sceneInfo,
                cancellationToken: cancellationToken
                );
        }

        /// <inheritdoc cref="ISavingSystem.RegisterUnityObjectAsync(GameObject, SceneInfo)"/>
        public static async UniTask<IDisposable> SavingSystemRegisterUnityObjectAsync(
            this GameObject source, 
            SceneInfo sceneInfo = default,
            CancellationToken cancellationToken = default
            )
        {
            return await SavingSystem.Self.RegisterUnityObjectAsync(
                gameObject: source,
                sceneInfo: sceneInfo,
                cancellationToken: cancellationToken
                );
        }

        /// <inheritdoc cref="ISavingSystem.RegisterUnityObjectAsync(Component, SceneInfo)"/>
        public static async UniTask<IDisposable> SavingSystemRegisterUnityObjectAsync(
            this Component source, 
            SceneInfo sceneInfo = default,
            CancellationToken cancellationToken = default
            )
        {
            return await SavingSystem.Self.RegisterUnityObjectAsync(
                component: source,
                sceneInfo: sceneInfo,
                cancellationToken: cancellationToken
                );
        }

        /// <inheritdoc cref="ISavingSystem.IsTypeRegistered(Type?)"/>
        public static bool SavingSystemIsTypeRegistered(this Type source)
        {
            return SavingSystem.Self.IsTypeRegistered(source);
        }

        /// <inheritdoc cref="ISavingSystem.IsTypeRegistered{T}()"/>
        public static bool SavingSystemIsTypeRegistered(this object source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.GetType().SavingSystemIsTypeRegistered();
        }

        /// <inheritdoc cref="ISavingSystem.IsInstanceRegistered(object?, SceneInfo)"/>
        public static bool SavingSystemIsInstanceRegistered(this object? source, SceneInfo sceneInfo = default)
        {
            return SavingSystem.Self.IsInstanceRegistered(source, sceneInfo);
        }
    }
}
