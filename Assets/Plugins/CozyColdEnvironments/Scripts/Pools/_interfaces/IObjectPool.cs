#nullable enable
using CCEnvs.Patterns.Commands;
using R3;
using System;

namespace CCEnvs.Pools
{
    public interface IObjectPool<T> : IObjectPoolBase<T>
        where T : class
    {
        /// <summary>
        /// -1 if <see cref="Preheat(FrameProvider?, int?, int?)"/> not started, otherwise value from 0 to 1
        /// </summary>
        float PreheatProgress { get; }

        PooledHandle<T> Get();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameProvider"></param>
        /// <param name="count"></param>
        /// <param name="batchSize"></param>
        /// <returns>A lazy factory of command which completed when <see cref="PreheatProgress"/> == 1</returns>
        IDisposable Preheat(
            FrameProvider? frameProvider,
            int? count = null,
            int? batchSize = null,
            int delayFrameBetweenBatchesCount = 0
            );
    }
}
