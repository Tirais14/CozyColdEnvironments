using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Init;
using UTIRLib.Linq;
using UTIRLib.Reflection;
using UTIRLib.Utils;

#nullable enable
namespace UTIRLib
{
    public static class SceneInitializer
    {
        /// <exception cref="TirLibException"></exception>
        public static void InitObject(IInitable initable)
        {
            if (initable.IsInited)
                throw new TirLibException($"{initable.GetTypeName()} is already inited.");

            initable.Init();
            TirLibDebug.PrintLog($"Inited => {initable.GetType().GetName()}.");
        }

        /// <exception cref="TirLibException"></exception>
        public static void InitObjects<T>()
            where T : IInitable
        {
            T[] initables = UnityObjectHelper.FindObjectsByType<T>(
                    FindObjectsInactive.Include).Where(x => x.IsNotNull())
                                                .ToArray();

            if (initables.CountNotDefault() == 0)
                throw new TirLibException($"Cannot find any {typeof(T).GetName()}.");

            for (int i = 0; i < initables.Length; i++)
                InitObject(initables[i]);
        }

        public static void InitObjects(IEnumerable<IInitable> initables)
        {
            foreach (var item in initables)
                InitObject(item);
        }

        public static void InitAllObjects()
        {
            IInitable[] inits =
                UnityObjectHelper.FindObjectsByType<IInitable>(FindObjectsInactive.Include);

            if (inits.IsEmpty())
            {
                TirLibDebug.PrintWarning("Nothing to init.");

                return;
            }

            Queue<IInitable> queue = CreateInitsQueue(inits);

            var loopPredicate = new LoopPredicate<int>(x => x > 0);
            IInitable initable;
            while (loopPredicate.Invoke(queue.Count))
            {
                initable = queue.Dequeue();

                if (initable.IsInited)
                    continue;

                InitObject(initable);
            }
        }

        private static IInitable[] GetFirstInits(IInitable[] inits)
        {
            return inits.Where(x => x.GetType().IsDefined<InitFirstAttribute>(inherit: true))
                        .ToArray();
        }

        private static (IInitable value, InitAfterTypeAttribute attribute)[] GetAfterTypeInits(
            IInitable[] inits)
        {
            return inits.Where(x => x.GetType().IsDefined<InitAfterTypeAttribute>(inherit: true))
                        .Select(x =>
                        {
                            var attribute = x.GetType().GetCustomAttribute<InitAfterTypeAttribute>();

                            return (x, attribute);
                        }).ToArray();
        }

        private static IInitable[] GetOtherInits(IInitable[] inits)
        {
            return inits.Where(x => !x.GetType().IsDefined<InitAttribute>(inherit: true))
                        .ToArray();
        }

        private static(IInitable value, InitAfterTypeAttribute attribute)[] 
            ResolveAfterTypeInits(
            IReadOnlyList<(IInitable value, InitAfterTypeAttribute attribute)> toProccess,
            IReadOnlyList<IInitable> proccessed
            )
        {
            var proccessedTypes = new HashSet<Type>(proccessed.Select(x => x.GetType()));

            return toProccess.Where(x => proccessedTypes.Contains(proccessedTypes))
                             .ToArray();
        }

        private static IInitable[] OrderAfterTypeInits(
            (IInitable value, InitAfterTypeAttribute attribute)[] predicated)
        {
            var toProccess = new List<(IInitable value, InitAfterTypeAttribute attribute)>(predicated);
            var proccessed = new List<IInitable>(predicated.Length);

            var loopPredicate = new LoopPredicate<int, int, int>
            {
                Predicate = (toProccessCount, proccessedCount, maxCount) =>
                {
                    return toProccessCount > 0
                           &&
                           proccessedCount < maxCount;
                }
            };

            (IInitable value, InitAfterTypeAttribute attribute)[] foundValues;
            while (loopPredicate.Invoke(toProccess.Count,
                                        proccessed.Count,
                                        predicated.Length))
            {
                foundValues = ResolveAfterTypeInits(toProccess, proccessed);

                //Takes last, if not found any
                if (foundValues.IsEmpty())
                {
                    proccessed.Add(toProccess[^1].value);
                    toProccess.RemoveAt(toProccess.Count - 1);

                    continue;
                }

                proccessed.AddRange(foundValues.Select(x => x.value).ToArray());
                toProccess.RemoveRange(foundValues);
            }

            return proccessed.ToArray();
        }

        private static void EnqueueFirstInits(Queue<IInitable> queue, IInitable[] inits)
        {
            IInitable[] firstInits = GetFirstInits(inits);

            if (firstInits.CountNotNull() == 0)
                throw new TirLibException("Not found any first initable.");

            for (int i = 0; i < firstInits.Length; i++)
                queue.Enqueue(firstInits[i]);
        }

        private static void EnqueueAfterTypeInits(Queue<IInitable> queue,
                                                   IInitable[] inits)
        {
            (IInitable value, InitAfterTypeAttribute attribute)[] predicatedInits =
                GetAfterTypeInits(inits);

            if (predicatedInits.IsEmpty())
                return;

            IInitable[] orderedPredicatedInits = OrderAfterTypeInits(predicatedInits);

            for (int i = 0; i < orderedPredicatedInits.Length; i++)
                queue.Enqueue(orderedPredicatedInits[i]);
        }

        private static void EnqueueOtherInits(Queue<IInitable> queue, IInitable[] inits)
        {
            IInitable[] other = GetOtherInits(inits);

            if (other.IsEmpty())
                return;

            for (int i = 0; i < other.Length; i++)
                queue.Enqueue(other[i]);
        }

        private static Queue<IInitable> CreateInitsQueue(IInitable[] inits)
        {
            var queue = new Queue<IInitable>(inits.Length);

            bool hasAttributed = inits.Any(x => x.GetType().IsDefined<InitAttribute>(inherit: true));
            if (hasAttributed)
            {
                EnqueueFirstInits(queue, inits);

                EnqueueAfterTypeInits(queue, inits);
            }

            EnqueueOtherInits(queue, inits);

            return queue;
        }
    }
}
