using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class GameObjectObserveHelper
    {
        public static Observable<Unit> OnDestroyAsObservable(this GameObject source)
        {
            CC.Guard.IsNotNullSource(source);

            return Observable.EveryUpdate(source.GetCancellationTokenOnDestroy());
        }
    }
}
