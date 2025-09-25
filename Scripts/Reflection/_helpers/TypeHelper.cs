using CCEnvs.Conversations;
using CCEnvs.Diagnostics;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Search;
using QuikGraph.Graphviz;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable

namespace CCEnvs.Reflection
{
    public static class TypeHelper
    {
        public static MemberMatches GetMemberMatches(Type left,
                                                     Type right)
        {
            CC.Validate.ArgumentNull(left, nameof(left));
            CC.Validate.ArgumentNull(right, nameof(right));

            BindingFlags bindings = BindingFlagsDefault.All;
            var comparer = new MemberEqualityComparer();
            FieldInfo[] valueFields = left.ForceGetFields(bindings);
            FieldInfo[] otherFields = right.ForceGetFields(bindings);

            var matches = new List<MemberInfo>();
            valueFields.Where(x => otherFields.FirstOrDefault(
                            y => comparer.Equals(x, y)).IsNotDefault())
                       .ForEach(x => matches.Add(x));

            PropertyInfo[] valueProps = left.ForceGetProperties(bindings);
            PropertyInfo[] otherProps = right.ForceGetProperties(bindings);

            valueProps.Where(x => otherProps.FirstOrDefault(
                            y => comparer.Equals(x, y)).IsNotDefault())
                       .ForEach(x => matches.Add(x));

            MethodInfo[] valueMethods = left.ForceGetMethods(bindings);
            MethodInfo[] otherMethods = right.ForceGetMethods(bindings);

            valueMethods.Where(x => otherMethods.FirstOrDefault(
                            y => comparer.Equals(x, y)).IsNotDefault())
                       .ForEach(x => matches.Add(x));

            EventInfo[] valueEvents = left.ForceGetMembers<EventInfo>(bindings);
            EventInfo[] otherEvents = right.ForceGetMembers<EventInfo>(bindings);

            valueEvents.Where(x => otherEvents.FirstOrDefault(
                            y => comparer.Equals(x, y)).IsNotDefault())
                       .ForEach(x => matches.Add(x));

            return new MemberMatches(
                valueFields.Length + valueProps.Length + valueMethods.Length + valueEvents.Length,
                otherFields.Length + otherProps.Length + otherMethods.Length + otherEvents.Length,
                matches.AsReadOnly());
        }

        [Obsolete("In developing")]
        /// <summary>
        /// Finds type with the largest number of base types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Type GetElderType(IEnumerable<Type>? types, Type? restriction = null)
        {
            throw new NotImplementedException("In developing");

            CC.Validate.CollectionArgument(types, nameof(types));

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

            //if (type.IsInterface)
            //    return GetInterfaceInheritancePath(type);

            return Collector.Collect(type, x => x.BaseType);
        }

        [Obsolete("In developing")]
        public static Queue<Type> GetInterfaceInheritancePath(Type type)
        {
            throw new NotImplementedException("In developing");

            CC.Validate.ArgumentNull(type, nameof(type));
            CC.Validate.Argument(type,
                                 nameof(type),
                                 type.IsInterface,
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