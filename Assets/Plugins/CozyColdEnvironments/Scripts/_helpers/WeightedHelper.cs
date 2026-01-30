#nullable enable
using CCEnvs.Collections;
using System.Collections.Generic;
using System;
using CommunityToolkit.Diagnostics;


#if UNITY_2017_1_OR_NEWER
using Random = UnityEngine.Random;
#endif

namespace CCEnvs
{
    public static class WeightedHelper
    {
        public static T ElementByWeightLinear<T>(
            this IEnumerable<T> source, 
            Func<T, float> weightSelector,
            int? randomSeed = null)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(weightSelector, nameof(weightSelector));

            if (source.IsEmpty())
                throw new ArgumentException($"Source is empty");

            float weightSum = 0f;

            foreach (var item in source)
                weightSum += weightSelector(item);

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
                weightSum += weightSelector(item);

                if (weightSum > targetWeight)
                    return item;
            }

            throw new InvalidOperationException();
        }
    }
}
