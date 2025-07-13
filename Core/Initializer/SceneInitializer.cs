using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Init;
using UTIRLib.Linq;
using UTIRLib.Utils;

#nullable enable
namespace UTIRLib
{
    public static class SceneInitializer
    {
        /// <exception cref="InvalidOperationException"></exception>
        public static void Init<T>()
            where T : IInitable
        {
            T[] initables = UnityObjectHelper.FindObjectsByType<T>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);

            int initablesCount = initables.Count(x => x.IsNotNull());
            if (initablesCount > 1)
                throw new InvalidOperationException("Cannot resolve initable to init.");
            else if (initablesCount == 0)
                throw new InvalidOperationException($"Cannot find initable {typeof(T)}.");

            initables.First(x => x.IsNotNull()).Init();
        }

        public static void InitAll()
        {
            IInitable[] initables =
            UnityObjectHelper.FindObjectsByType<IInitable>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            InitAttributed(initables);

            IEnumerable<IInitable> basicInitables = initables.Where(
                x => !x.GetType()
                     .IsDefined(typeof(InitAttribute),
                                inherit: true));

            InitBasic(initables);
        }

        private static IInitable[] OrderAttributedInitables(
            IInitable firstInitable,
            IEnumerable<IInitable> attributed)
        {
            var toProccess = new List<IInitable>(attributed);
            var proccessed = new List<IInitable>(toProccess.Count + 1) { firstInitable };

            var loopPredicate = new LoopPredicate(
                () => proccessed.Count < toProccess.Count + proccessed.Count,
                throwMessage: $"Cannot resolve next initable. {proccessed[^1].GetProccessedTypeName()}");

            while (loopPredicate.Invoke())
            {
                IInitable? next = toProccess.Find(
                                  x => GetNextAttributedInitable(x, proccessed[^1]));

                if (next.IsNull())
                    next = toProccess[0];

                proccessed.Add(next);
                toProccess.Remove(next);

            }

            return proccessed.ToArray();
        }

        private static bool GetNextAttributedInitable(IInitable initable, IInitable other)
        {
            var otherAttribute = other.GetType().GetCustomAttribute<InitAfterAttribute>();

            return initable.GetType() == otherAttribute.Type;
        }

        private static IInitable? GetFirstInitable(IInitable[] initables)
        {
            return initables.SingleOrDefault(
                        x => x.GetType()
                             .IsDefined(typeof(InitFirstAttribute),
                                        inherit: true));
        }

        private static IEnumerable<IInitable> FilterByAttribute(IInitable[] initables)
        {
            return initables.Where(x => x.GetType().
                                          IsDefined(typeof(InitAfterAttribute),
                                                    inherit: true));
        }

        private static void InitAttributed(IInitable[] initables)
        {
            IInitable? firstInitable = GetFirstInitable(initables);
            IEnumerable<IInitable> attributedInitables = FilterByAttribute(initables);

            if (firstInitable.IsNull())
            {
                if (attributedInitables.IsNotEmpty())
                    throw new InvalidOperationException("Cannot find first initable.");
                else
                    return;
            }

            IInitable[] ordered = OrderAttributedInitables(firstInitable, attributedInitables);

            for (int i = 0; i < ordered.Length; i++)
            {
                ordered[i].Init();

                TirLibDebug.Log($"Inited: {ordered[i].GetType().GetProccessedName()}.");
            }
        }

        private static void InitBasic(IInitable[] initables)
        {
            IEnumerable<IInitable> basicInitables = initables.Where(
                x => !x.GetType()
                     .IsDefined(typeof(InitAttribute),
                                inherit: true));

            foreach (var initable in basicInitables)
            {
                initable.Init();

                TirLibDebug.Log($"Inited: {initable.GetType().GetProccessedName()}.");
            }
        }
    }
}
