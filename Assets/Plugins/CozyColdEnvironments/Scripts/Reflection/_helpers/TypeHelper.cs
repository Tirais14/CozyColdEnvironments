using CCEnvs.Diagnostics;
using QuikGraph;
using QuikGraph.Algorithms.Search;
using QuikGraph.Graphviz;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace CCEnvs.Reflection
{
    public static class TypeHelper
    {
        /// <summary>
        /// Also supports interfaces
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<Type> CollectBaseTypes(this Type type)
        {
            CC.Guard.IsNotNull(type, nameof(type));

            //if (type.IsInterface)
            //    return GetInterfaceInheritancePath(type);

            return Do.Collect(type, x => x.BaseType);
        }

        [Obsolete("In developing", error: true)]
        public static Queue<Type> GetInterfaceInheritancePath(Type type)
        {
            throw new NotImplementedException("In developing");

            CC.Guard.IsNotNull(type, nameof(type));
            CC.Guard.ArgumentObsolete(type,
                                 nameof(type),
                                 type.IsInterface,
                                 "Type is not interface");

            var graph = new AdjacencyGraph<Type, Edge<Type>>();

            ////root
            //graph.AddVertex(type);

            //Get all interfaces and binding to source type
            Queue<(Type? source, Type target)> allIntefacePairs = Do.Collect(
                (source: (Type?)null, target: type), pair =>
            {
                return pair.target.GetInterfaces()
                                  .Select(x => (source: (Type?)pair.target, target: x))
                                  .DistinctBy(x => HashCode.Combine(x.source, x.target))
                                  .ToArray();
            });

            var sb = new StringBuilder();
            foreach (var (source, target) in allIntefacePairs)
            {
                sb.AppendLine($"Source: {source?.GetName()}. Target = {target?.GetName()}.");
            }

            CCDebug.PrintLog(sb.ToString(), typeof(TypeHelper));

            //Creating graph
            foreach (var ifacePair in allIntefacePairs)
            {
                if (ifacePair.IsDefault())
                    continue;

                if (ifacePair.target is not null)
                {
                    graph.AddVertex(ifacePair.target);

                    if (ifacePair.source is not null)
                        graph.AddEdge(new Edge<Type>(ifacePair.source, ifacePair.target));
                }
            }

            var algorithm = new GraphvizAlgorithm<Type, Edge<Type>>(graph);
            CCDebug.PrintWarning(algorithm.Generate());

            var bfs = new BreadthFirstSearchAlgorithm<Type, Edge<Type>>(graph);
            var pathParts = new List<(Type, Type)>();

            bfs.TreeEdge += (x) => pathParts.Add((x.Source, x.Target));

            bfs.Compute();

            return new Queue<Type>();

            //===========================================
            static double edgeCost(Edge<Type> edge) => 1d;

            static Queue<Type> reconstructPath(IEnumerable<Edge<Type>> rawPath)
            {
                var results = new Queue<Type>();
                foreach (var item in rawPath)
                {
                    results.Enqueue(item.Source);
                    results.Enqueue(item.Target);
                }

                return results;
            }
        }

        /// <exception cref="TypeNotFoundException"></exception>
        public static Type GetPirmitiveType(string shortName, bool throwOnError = true)
        {
            switch (shortName)
            {
                case "byte":
                    return typeof(byte);
                case "sbyte":
                    return typeof(sbyte);
                case "short":
                    return typeof(short);
                case "ushort":
                    return typeof(ushort);
                case "int":
                    return typeof(int);
                case "uint":
                    return typeof(uint);
                case "long":
                    return typeof(long);
                case "ulong":
                    return typeof(ulong);
                case "string":
                    return typeof(string);
                case "bool":
                    return typeof(bool);
                case "object":
                    return typeof(object);
                default:
                    {
                        if (throwOnError)
                            throw new TypeNotFoundException(shortName, "Type hasn't special short name.");
                        return null!;
                    }
            }
        }
    }
}