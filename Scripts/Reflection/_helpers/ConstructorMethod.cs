using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class ConstructorMethod
    {
        private readonly static Dictionary<Type, Type[]> constructorTypes = new();
        private readonly static Dictionary<Type, MethodInfo[]> constructorMethods = new();

        public static Type[] GetConstructorTypes(Type constructableType)
        {
            Validate.ArgumentNull(constructableType, nameof(constructableType));

            if (constructorTypes.TryGetValue(constructableType, out Type[] results))
                return results;

            results = TypeFinder.FindTypesInAppDomain(new TypeFinderParameters
            {
                DefinedAttributeTypes = CC.C.Array(typeof(ConstructorOfTypeAttribute))
            });

            constructorTypes.Add(constructableType, results);

            return results;
        }

        public static MethodInfo[] Get(Type constructableType)
        {
            if (constructorMethods.TryGetValue(constructableType, out MethodInfo[] results))
                return results;

            HashSet<MethodInfo> resultCollection =
                (from type in GetConstructorTypes(constructableType)
                 select type.ForceGetMethods(BindingFlagsDefault.StaticAll) into methods
                 from method in methods
                 where method.ReturnType.IsType(constructableType)
                 ||
                 method.IsDefined<ConstructorAttribute>(inherit: true)
                 select method)
                 .ToHashSet();

            resultCollection = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                       select assembly.GetTypes() into types
                       from type in types
                       select type.ForceGetMethods(BindingFlagsDefault.StaticAll) into methods
                       from method in methods
                       where method.ReturnType.IsType(constructableType)
                       ||
                       method.IsDefined<ConstructorAttribute>(inherit: true)
                       select method)
                       .Aggregate(resultCollection, TryAddToResults);

            results = resultCollection.ToArray();
            constructorMethods.Add(constructableType, results);

            return results;

            static HashSet<MethodInfo> TryAddToResults(HashSet<MethodInfo> collection,
                                                       MethodInfo method)
            {
                if (collection.Contains(method))
                    collection.Add(method);

                return collection;
            }
        }
    }
}
