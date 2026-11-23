using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization.SmartFormat.Utilities;

#nullable enable
namespace CCEnvs.UnityX.Components
{
    public sealed class CoroutineBehaviour : CCBehaviour
    {
        private static readonly ObjectPool<List<object>> listPool = new(Factory.Create(() => new List<object>(16)), capacity: 4, maxSize: 128);

        public void Action(Action<List<object>> coroutine)
        {
            Guard.IsNotNull(coroutine, nameof(coroutine));

            using var list = listPool.Get();
            coroutine(list.Value);
            StartCoroutine(ListToCoroutine(list.Value));
        }

        private IEnumerator ListToCoroutine(List<object> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var obj = list[i];

                switch (obj)
                {
                    case Action action:
                        action();
                        break;
                    case WaitForFrames waitForFrames:
                        while (waitForFrames.frameCount-- >= 1)
                            yield return null;
                        break;
                    default:
                        break;
                }

                yield return obj;
            }
        }
    }
}
