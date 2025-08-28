using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace UTIRLib.Unity
{
#nullable enable
    public record RemoveComponentsArguments
    {
        private HashSet<Type> typesToRemove = null!;
        private HashSet<Type> typesToExclude = null!;

        public GameObject Object { get; set; } = null!;
        public Type[] TypesToRemove {
            get => typesToRemove.ToArray();
            set => typesToRemove = new HashSet<Type>(value);
        }
        public Type[] TypesToExclude {
            get => typesToRemove.ToArray();
            set => typesToExclude = new HashSet<Type>(value);
        }
        public string[] NamespacesToRemove { get; set; } = Array.Empty<string>();
        public string[] NamespacesToExclude { get; set; } = Array.Empty<string>();

        public bool IsToRemoveType(Type? type)
        {
            if (type is null)
                return false;

            bool removeByType = typesToRemove.IsNotNullOrEmpty() 
                                &&
                                typesToRemove.Contains(type);

            bool removeByNamespace = NamespacesToRemove.IsNotNullOrEmpty()
                                     &&
                                     IsNamespaceToRemove(type.Namespace);

            return (removeByType
                   ||
                   removeByNamespace)
                   &&
                   !IsExcludeType(type);
        }

        public bool IsExcludeType(Type? type)
        {
            if (type is null)
                return false;

            bool excludeByType = typesToExclude.IsNotNullOrEmpty() 
                                 &&
                                 typesToExclude.Contains(type);

            bool excludeByNamepsace = NamespacesToRemove.IsNotNullOrEmpty()
                                      &&
                                      IsNamespaceToExclude(type.Namespace);

            return excludeByType || excludeByNamepsace;
        }

        private bool IsNamespaceToRemove(string ns)
        {
            return NamespacesToRemove.Any(x => ns.Contains(x));
        }

        private bool IsNamespaceToExclude(string ns)
        {
            return NamespacesToExclude.Any(x => ns.Contains(x));
        }
    }
}
