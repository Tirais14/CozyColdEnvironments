using CCEnvs.Converting;
using CCEnvs.Diagnostics;
using QuikGraph;
using QuikGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using SuperLinq;
using System.Runtime.CompilerServices;
using System.Text;
using QuikGraph.Graphviz;

#nullable enable

namespace CCEnvs.Reflection
{
    public static class TypeHelper
    {
        /// <summary>
        /// Finds type with the largest number of base types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Type GetElderType(IEnumerable<Type>? types, Type? restriction = null)
        {
            CC.Validate.CollectionArgument(nameof(types), types);

            IEnumerable<Type> ordered = from type in types
                                        orderby type.GetParentsCount() descending
                                        select type;

            if (restriction is not null)
                return ordered.FirstOrDefault(x => x.IsType(restriction))
                       ??
                       throw new LogicException("Incorrect input types. Not found any matches by setted restriction.");

            return ordered.First();
        }

        /// <summary>
        /// Also supports interfaces
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<Type> CollectBaseTypes(Type type)
        {
            CC.Validate.ArgumentNull(type, nameof(type));

            if (type.IsInterface)
                return GetInterfaceBaseTypes(type);

            return Collector.Collect(type, x => x.BaseType);
        }

        public static Queue<Type> GetInterfaceBaseTypes(Type type)
        {
            CC.Validate.ArgumentNull(type, nameof(type));
            CC.Validate.Argument(type,
                                 nameof(type),
                                 x => type.IsInterface,
                                 "Type is not interface");

            var graph = new AdjacencyGraph<Type, Edge<Type>>();

            ////root
            //graph.AddVertex(type);

            //Get all interfaces and binding to source type
            Queue<(Type? source, Type target)> allIntefacePairs = Collector.Collect(
                (source: (Type?)null, target: type), pair =>
            {
                return pair.target.GetInterfaces()
                                  .Select(x => (source: (Type?)pair.target, target: x))
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

            TryFunc<Type, IEnumerable<Edge<Type>>> tryFindPath = graph.ShortestPathsDijkstra(edgeCost, type);

            if (tryFindPath(type, out IEnumerable<Edge<Type>> rawPath))
            {
                CCDebug.PrintLog($"Path for interface type = {type.GetName()} is constructed.", DebugContext.Additive(typeof(TypeHelper)));

                return reconstructPath(rawPath);
            }

            var algorithm = new GraphvizAlgorithm<Type, Edge<Type>>(graph);
            CCDebug.PrintWarning(algorithm.Generate());


            CCDebug.PrintWarning($"Path for interface type = {type.GetName()} is not constructed.", DebugContext.Additive(typeof(TypeHelper)));
            return new Queue<Type>();

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