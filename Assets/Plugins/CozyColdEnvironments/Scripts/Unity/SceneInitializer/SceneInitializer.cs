using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CCEnvs.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable enable
#pragma warning disable S127
namespace CCEnvs.Unity.Initables
{
    public static class SceneInitializer
    {
        /// <exception cref="CCException"></exception>
        public static void InitObject(IInitable initable)
        {
            if (initable.IsInited)
                throw new CCException($"{initable.GetTypeName()} is already inited.");

            initable.Init();
            SetInited(initable);

            CCDebug.PrintLog($"Inited => {initable.GetType().GetName()}.");
        }
        /// <exception cref="CCException"></exception>
        public static async UniTask InitObjectAsync(IInitableAsync initableAsync)
        {
            try
            {
                if (initableAsync.IsInited)
                    throw new CCException($"{initableAsync.GetTypeName()} is already inited.");

                UniTask task = initableAsync.InitAsync();
                CCEnvs.CC.NeccesaryTasks.RegisterTask(task);
                await task;

                SetInited(initableAsync);

                CCDebug.PrintLog($"Inited => {initableAsync.GetType().GetName()}.");
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }

        ///// <exception cref="TirLibException"></exception>
        //public static void InitObjects<T>()
        //    where T : IInitable
        //{
        //    T[] initables = UnityObjectHelper.FindObjectsByType<T>(
        //            FindObjectsInactive.Include).Where(x => x.IsNotNull())
        //                                        .ToArray();

        //    if (initables.CountNotDefault() == 0)
        //        throw new TirLibException($"Cannot find any {typeof(T).GetName()}.");

        //    for (int i = 0; i < initables.Length; i++)
        //        InitObject(initables[i]);
        //}

        //public static void InitObjects(IEnumerable<IInitable> initables)
        //{
        //    foreach (var item in initables)
        //        InitObject(item);
        //}

        public static async UniTask InitAllObjectsAsync(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include)
        {
            var allInitables = new ConcurrentDictionary<Type, IInitableBase>();

            IInitable[] inits =
                FindInScene.Q.IncludeInactive().Components<IInitable>()
                .Where(x => x.GetType().IsDefinedAny(inherit: true,
                                                     typeof(InitAttribute),
                                                     typeof(InitFirstAttribute),
                                                     typeof(InitAfterTypeAttribute))
                                                     ).ToArray();

            IInitableAsync[] initsAsync =
                FindInScene.Q.IncludeInactive().Components<IInitableAsync>()
                .Where(x => x.GetType().IsDefinedAny(inherit: true,
                                                     typeof(InitAsyncAttribute),
                                                     typeof(InitAsyncFirstAttribute),
                                                     typeof(InitAsyncAfterTypeAttribute))
                                                     ).ToArray();

            AddToAllInitables(inits, allInitables);
            AddToAllInitables(initsAsync, allInitables);

            if (inits.Length > 0)
                DoInitInitables(inits);

            if (initsAsync.Length > 0)
                await DoInitInitablesAsync(initsAsync, allInitables);
        }

        /// <exception cref="InvalidOperationException"></exception>
        private static void SetInited(IInitableBase initable)
        {
            PropertyInfo[] props = initable.GetType()
                                           .Reflect()
                                           .NonPublic()
                                           .IncludeBaseTypes()
                                           .Properties()
                                           .ToArray();

            if (props.IsEmpty())
                throw new Exception("Cannot find any property.");

            PropertyInfo? isInitedProp = props.FirstOrDefault(
                x => x.Name == nameof(IInitableBase.IsInited)
                &&
                x.PropertyType == typeof(bool)
                )
                ??
                throw new MemberNotFoundException(MemberTypes.Property, initable.GetType(), nameof(IInitableBase.IsInited));

            if (isInitedProp.SetMethod is null)
                throw new InvalidOperationException("Not found SetMethod.");

            isInitedProp.SetValue(initable, true);
        }

        private static void AddToAllInitables(IReadOnlyList<IInitableBase> inits,
            ConcurrentDictionary<Type, IInitableBase> allInitables)
        {
            IInitableBase initable;
            int count = inits.Count;
            for (int i = 0; i < count; i++)
            {
                initable = inits[i];
                allInitables.TryAdd(initable.GetType(), initable);
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
                            InitAfterTypeAttribute attribute = x.GetType().GetCustomAttribute<InitAfterTypeAttribute>();

                            return (x, attribute);
                        }).ToArray();
        }

        private static IInitable[] GetOtherInits(IInitable[] inits)
        {
            return inits.Where(x => !x.GetType().IsDefined<InitAttribute>(inherit: true))
                        .ToArray();
        }

        private static (IInitable value, InitAfterTypeAttribute attribute)[]
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

            var loopPredicate = new LoopFuse<int, int, int>
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
                throw new CCException("Not found any first initable.");

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

        private static void DoInitInitables(IInitable[] inits)
        {
            Queue<IInitable> queue = CreateInitsQueue(inits);
            IInitable initable;
            var loopPredicate = new LoopFuse<int>(x => x > 0);
            while (loopPredicate.Invoke(queue.Count))
            {
                initable = queue.Dequeue();

                if (initable.IsInited)
                    continue;

                InitObject(initable);
            }
        }

        private static IInitableAsync[] GetInitsAsyncFirst(
            List<IInitableAsync> initablesAsync)
        {
            var results = new List<IInitableAsync>(initablesAsync.Count);

            IInitableAsync initableAsync;
            for (int i = 0; i < initablesAsync.Count;)
            {
                initableAsync = initablesAsync[i];
                if (!initableAsync.IsInited
                    &&
                    initableAsync.GetType().IsDefined<InitAsyncFirstAttribute>())
                {
                    results.Add(initableAsync);
                    initablesAsync.RemoveAt(i);
                }
                else i++;
            }

            return results.ToArray();
        }

        private static (IInitableAsync initableAsync, InitAsyncAfterTypeAttribute attribute)[]
            GetInitsAsyncAfterType(List<IInitableAsync> initablesAsync)
        {
            var initablesFirstAsync = new List<(IInitableAsync initableAsync, InitAsyncAfterTypeAttribute attribute)>(initablesAsync.Count);

            IInitableAsync initableAsync;
            for (int i = 0; i < initablesAsync.Count;)
            {
                initableAsync = initablesAsync[i];
                if (!initableAsync.IsInited
                    &&
                    initableAsync.GetType()
                    .GetCustomAttribute<InitAsyncAfterTypeAttribute>()
                    .Is<InitAsyncAfterTypeAttribute>(out InitAsyncAfterTypeAttribute? attribute))
                {
                    initablesFirstAsync.Add((initablesAsync[i], attribute));
                    initablesAsync.RemoveAt(i);
                }
                else i++;
            }

            return initablesFirstAsync.ToArray();
        }

        private static IInitableAsync[] GetOtherInitsAsync(
            List<IInitableAsync> initablesAsync)
        {
            return initablesAsync.Where(x => !x.IsInited).ToArray();
        }

        private static async UniTask StartInitsAsyncFirst(
            IInitableAsync[] initablesAsyncFirst)
        {
            int count = initablesAsyncFirst.Length;
            for (int i = 0; i < count; i++)
                await InitObjectAsync(initablesAsyncFirst[i]);
        }

        /// <exception cref="ArgumentException"></exception>
        private static async UniTask InitAfterTypeAsync(
            IInitableAsync initableAsyncMain, Type[] types,
            ConcurrentDictionary<Type, IInitableBase> allInitablesAsync)
        {
            try
            {
                if (allInitablesAsync.IsEmpty())
                    throw new ArgumentException("Cannot be empty");

                //Checks for types and move to specified array for performance
                int observableInitablesIdx = 0;
                var observableInitables = new IInitableBase[types.Length];
                for (int i = 0; i < types.Length; i++)
                {
                    if (!allInitablesAsync.TryGetValue(types[i], out IInitableBase? temp))
                        throw new ArgumentException($"Cannot find type {types[i].GetName()}.");

                    observableInitables[observableInitablesIdx++] = temp;
                }

                while (!initableAsyncMain.IsInited)
                {
                    if (observableInitables.Any(x => !x.IsInited))
                        await UniTask.Yield();
                    else
                    {
                        await InitObjectAsync(initableAsyncMain);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }

        private static void StartInitsAsyncAfterType(
            (IInitableAsync initable, InitAsyncAfterTypeAttribute attribute)[] initablesAfterTypeAsync,
            ConcurrentDictionary<Type, IInitableBase> allInitablesAsync)
        {
            try
            {
                (IInitableAsync initable, InitAsyncAfterTypeAttribute attribute) item;
                int count = initablesAfterTypeAsync.Length;
                for (int i = 0; i < count; i++)
                {
                    item = initablesAfterTypeAsync[i];
                    UniTask.RunOnThreadPool(
                        () => InitAfterTypeAsync(
                            item.initable,
                            item.attribute.InitableTypes,
                            allInitablesAsync))
                                .Forget(ex => CCDebug.PrintException(ex));
                }
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }

        private static async UniTask StartInitsAsyncOther(
            IInitableAsync[] otherInitablesAsync)
        {
            try
            {
                int count = otherInitablesAsync.Length;
                for (int i = 0; i < count; i++)
                    await InitObjectAsync(otherInitablesAsync[i]);
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }

        private static async UniTask DoInitInitablesAsync(IInitableAsync[] initsAsync,
            ConcurrentDictionary<Type, IInitableBase> allInitables)
        {
            List<IInitableAsync> initsAsyncList = initsAsync.ToList();

            IInitableAsync[] initsAsyncFirst = GetInitsAsyncFirst(initsAsyncList);
            (IInitableAsync initableAsync, InitAsyncAfterTypeAttribute attribute)[] initsAsyncAfterType
                = GetInitsAsyncAfterType(initsAsyncList);
            IInitableAsync[] initsAsyncOther = GetOtherInitsAsync(initsAsyncList);

            await StartInitsAsyncFirst(initsAsyncFirst);
            StartInitsAsyncAfterType(initsAsyncAfterType, allInitables);
            await StartInitsAsyncOther(initsAsyncOther);
        }
    }
}
