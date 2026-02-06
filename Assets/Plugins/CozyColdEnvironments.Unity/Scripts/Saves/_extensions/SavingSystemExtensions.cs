using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public static class SavingSystemExtensions
    {
        /// <inheritdoc cref="ISavingSystem.RegisterObject{TObject}(TObject, string, SceneInfo?)"/>
        public static IDisposable SavingSystemRegisterObject<TObject>(
            this TObject source,
            string key,
            SceneInfo sceneInfo = default
            )
            where TObject : class
        {
            return SavingSystem.Self.RegisterObject(
                obj: source,
                key: key,
                sceneInfo: sceneInfo
                );
        }

        /// <inheritdoc cref="ISavingSystem.RegisterObject{TObject}(TObject, Func{TObject, string}, SceneInfo?)"/>
        public static IDisposable SavingSystemRegisterObject<TObject>(
            this TObject source,
            Func<TObject, string> keySelector,
            SceneInfo sceneInfo = default
            )
            where TObject : class
        {
            return SavingSystem.Self.RegisterObject(
                obj: source,
                keySelector: keySelector,
                sceneInfo: sceneInfo
                );
        }

        /// <inheritdoc cref="ISavingSystem.RegisterObject{TObject, TState}(TObject, TState, Func{TObject, TState, string}, SceneInfo?)"/>
        public static IDisposable SavingSystemRegisterObject<TObject, TState>(
            this TObject source,
            TState state,
            Func<TObject, TState, string> keySelector,
            SceneInfo sceneInfo = default
            )
            where TObject : class
        {
            return SavingSystem.Self.RegisterObject(
                source,
                state,
                keySelector,
                sceneInfo: sceneInfo
                );
        }

        /// <inheritdoc cref="ISavingSystem.RegisterUnityObjectAsync(GameObject, SceneInfo)"/>
        public static IDisposable SavingSystemRegisterUnityObject(
            this GameObject source, 
            SceneInfo sceneInfo = default
            )
        {
            return SavingSystem.Self.RegisterUnityObject(
                gameObject: source,
                sceneInfo: sceneInfo
                );
        }

        /// <inheritdoc cref="ISavingSystem.RegisterUnityObjectAsync(Component, SceneInfo)"/>
        public static IDisposable SavingSystemRegisterUnityObject(
            this Component source, 
            SceneInfo sceneInfo = default
            )
        {
            return SavingSystem.Self.RegisterUnityObject(
                component: source,
                sceneInfo: sceneInfo
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
