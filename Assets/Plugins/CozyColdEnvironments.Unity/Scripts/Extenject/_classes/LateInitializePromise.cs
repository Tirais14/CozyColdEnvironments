#if UNITASK_PLUGIN
using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using Zenject;

#nullable enable
namespace CCEnvs.Zenject
{
    public readonly struct LateInitializePromise : IInitializable
    {
        private readonly IStartable target;

        public LateInitializePromise(
            IStartable target
            )
        {
            Guard.IsNotNull(target);

            this.target = target;
        }

        void IInitializable.Initialize()
        {
            var systems = PlayerLoop.GetCurrentPlayerLoop().subSystemList.Find(x => x.type == typeof(Initialization))
                .Maybe()
                .Map(initLoop => initLoop.subSystemList)
                .GetValue();
            //UniTask.Create((@this: this, cancellationToken),
            //    static async (args) =>
            //    {
            //        try
            //        {
            //            await UniTask.Yield(
            //                PlayerLoopTiming.LastInitialization,
            //                args.cancellationToken
            //                );

            //            args.@this.target.LateInitialize();
            //        }
            //        catch (System.Exception ex)
            //        {
            //            args.@this.PrintException(ex);
            //            throw;
            //        }
            //    })
            //    .Timeout(5.Minutes(), DelayType.UnscaledDeltaTime)
            //    .Forget();
        }
    }
}
#endif