#nullable enable
using CCEnvs.Collections;
using System.Collections.Generic;
using System;

#if UNITY_2017_1_OR_NEWER
using Random = UnityEngine.Random;
#endif

namespace CCEnvs
{
    public static class WeightedHelper
    {
        public static T ElementByWeightLinear<T>(this IEnumerable<T> source, int? randomSeed = null)
            where T : IWeighted
        {
            CC.Guard.IsNotNullSource(source);

            if (source.IsEmpty())
                throw new ArgumentException($"Source is empty");

            float weightSum = 0f;

            foreach (var item in source)
                weightSum += item.Weight;

            float targetWeight;
#if UNITY_2017_1_OR_NEWER

            if (randomSeed is not null)
                Random.InitState(randomSeed.Value);

            targetWeight = Random.Range(0, weightSum);
#else

            var r = randomSeed is null ? new Random() : new Random(randomSeed.Value);
            targetWeight = r.NextFloat(0, weightSum);

#endif
            weightSum = 0f;

            foreach (var item in source)
            {
                weightSum += item.Weight;

                if (weightSum > targetWeight)
                    return item;
            }

            throw new InvalidOperationException();
        }
    }
}
