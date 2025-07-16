using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Attributes;
using UTIRLib.Diagnostics;
using UTIRLib.Init;
using UTIRLib.Linq;
using UTIRLib.Utils;

#nullable enable
namespace UTIRLib
{
    public static class SceneInitializer
    {
        /// <exception cref="ArgumentException"></exception>
        public static void InitObject(IInitable initable)
        {
            if (initable.IsInited)
                throw new ArgumentException($"{initable.GetProccessedTypeName()} is already inited.");

            initable.Init();
            TirLibDebug.Log($"Inited => {initable.GetType().GetProccessedName()}.");
        }

        /// <exception cref="InvalidOperationException"></exception>
        public static void InitObject<T>()
            where T : IInitable
        {
            T[] initables = UnityObjectHelper.FindObjectsByType<T>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);

            int initablesCount = initables.Count(x => x.IsNotNull());
            if (initablesCount > 1)
                throw new InvalidOperationException("Many instances. Cannot resolve initable to init.");
            else if (initablesCount == 0)
                throw new InvalidOperationException($"Cannot find initable {typeof(T).GetProccessedName()}.");

            InitObject(initables.First(x => x.IsNotNull()));
        }

        public static void InitObjects(IEnumerable<IInitable> initables)
        {
            foreach (var item in initables)
                InitObject(item);
        }

        public static void InitAllObjects()
        {
            IInitable[] initables =
                UnityObjectHelper.FindObjectsByType<IInitable>(FindObjectsInactive.Include);

            if (initables.IsEmpty())
            {
                TirLibDebug.Warning("Nothing to init.");

                return;
            }

            IInitable[] attributedInits = GetAttributedInits(initables);

            Queue<IInitable> queue = CreateInitsQueue(attributedInits);

            var loopPredicate = new LoopPredicate<int>(x => x > 0);
            IInitable initable;
            while (loopPredicate.Invoke(queue.Count))
            {
                initable = queue.Dequeue();

                InitObject(initable);
            }
        }

        private static IInitable[] GetAttributedInits(IInitable[] inits)
        {
            return inits.Where(x => x.GetType().IsDefined<InitAttribute>(inherit: true))
                        .ToArray();
        }

        private static IInitable[] GetFirstInits(IInitable[] inits)
        {
            return inits.Where(x => x.GetType().IsDefined<InitFirstAttribute>(inherit: true))
                        .ToArray();
        }

        private static (IInitable value, InitAfterAttribute attribute)[] GetPredicatedInits(
            IInitable[] inits)
        {
            return inits.Where(x => x.GetType().IsDefined<InitAfterAttribute>(inherit: true))
                        .Select(x =>
                        {
                            var attribute = x.GetType().GetCustomAttribute<InitAfterAttribute>();

                            return (x, attribute);
                        }).ToArray();
        }

        private static (IInitable value, InitAfterAttribute attribute)[] ResolvePredicatedInits(
            IReadOnlyList<(IInitable value, InitAfterAttribute attribute)> toProccess,
            IReadOnlyList<IInitable> proccessed)
        {
            var proccessedTypes = new HashSet<Type>(proccessed.Select(x => x.GetType()));

            return toProccess.Where(x => proccessedTypes.Contains(proccessedTypes))
                             .ToArray();
        }

        private static IInitable[] OrderPredicatedInits(
            (IInitable value, InitAfterAttribute attribute)[] predicated,
            IInitable comparable)
        {
            var toProccess = new List<(IInitable value, InitAfterAttribute attribute)>(predicated);
            var proccessed = new List<IInitable>(predicated.Length);

            var loopPredicate = new LoopPredicate<int, int, int>(
                (toProccessCount, proccessedCount, maxCount) => toProccessCount > 0 
                                                                && 
                                                                proccessedCount < maxCount);

            (IInitable value, InitAfterAttribute attribute)[] foundValues;
            while (loopPredicate.Invoke(toProccess.Count,
                                        proccessed.Count,
                                        predicated.Length))
            {
                foundValues = ResolvePredicatedInits(toProccess, proccessed);

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

        private static void EnqueuePredicatedInits(Queue<IInitable> queue,
                                                   IInitable[] inits)
        {
            (IInitable value, InitAfterAttribute attribute)[] predicatedInits =
                GetPredicatedInits(inits);

            if (predicatedInits.IsEmpty())
                return;

            IInitable[] orderedPredicatedInits = OrderPredicatedInits(predicatedInits,
                                                                      queue.Peek());

            for (int i = 0; i < orderedPredicatedInits.Length; i++)
                queue.Enqueue(orderedPredicatedInits[i]);
        }

        private static Queue<IInitable> CreateInitsQueue(IInitable[] attributed)
        {
            if (attributed.IsEmpty()) 
                return new Queue<IInitable>(0);

            var queue = new Queue<IInitable>(attributed.Length);

            EnqueueFirstInits(queue, attributed);

            EnqueuePredicatedInits(queue, attributed);

            return queue;
        }
    }
}
