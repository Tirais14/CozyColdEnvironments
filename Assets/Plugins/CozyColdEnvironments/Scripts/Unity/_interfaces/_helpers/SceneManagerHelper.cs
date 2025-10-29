using CCEnvs.Disposables;
using CCEnvs.Reflection;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Scenes
{
    public static class SceneManagerHelper
    {
        public static ISubscription BindToSceneLoaded<TDelegate>(TDelegate func)
            where TDelegate : Delegate
        {
            CC.Guard.IsNotNull(func, nameof(func));

            Reflected rSceneManager = typeof(SceneManager).AsReflected();
            ContextedEventInfo<UnityAction<Scene, LoadSceneMode>> sceneLoadedEvent = rSceneManager.Event<UnityAction<Scene, LoadSceneMode>>(nameof(SceneManager.sceneLoaded));

            return Subscription.FromEvent((_, _) => func.DynamicInvoke(), sceneLoadedEvent);
        }

        public static ISubscription BindToActiveSceneChanged<TDelegate>(TDelegate func)
            where TDelegate : Delegate
        {
            CC.Guard.IsNotNull(func, nameof(func));

            Reflected rSceneManager = typeof(SceneManager).AsReflected();
            ContextedEventInfo<UnityAction<Scene, Scene>> activeSceneChangedEvent = rSceneManager.Event<UnityAction<Scene, Scene>>(nameof(SceneManager.activeSceneChanged));

            return Subscription.FromEvent((_, _) => func.DynamicInvoke(), activeSceneChangedEvent);
        }
    }
}
