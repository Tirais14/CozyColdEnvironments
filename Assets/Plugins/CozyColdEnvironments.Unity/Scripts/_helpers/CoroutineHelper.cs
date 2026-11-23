using CCEnvs.UnityX.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX
{
    public static class CoroutineHelper
    {
        private static readonly WeakReference<CoroutineBehaviour?> behaviour = new(null);

        public static void Action(Action<List<object>> coroutine)
        {
            GetOrCreateBehaviour().Action(coroutine);
        }

        private static CoroutineBehaviour GetOrCreateBehaviour()
        {
            if (!behaviour.TryGetTarget(out var beh) 
                || beh == null)
            {
                var go = new GameObject("coroutine-behaviour");
                UnityEngine.Object.DontDestroyOnLoad(go);
                beh = go.AddComponent<CoroutineBehaviour>();
                behaviour.SetTarget(beh);
            }

            return beh;
        }
    }
}
