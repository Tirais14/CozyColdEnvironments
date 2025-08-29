using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.Unity
{
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

            bool exclude = IsExcludeType(type);
            bool removeByType = typesToRemove.IsNotNullOrEmpty() 
                                &&
                                typesToRemove.Any(x => type.IsType(x));
            if (!exclude && removeByType)
                return true;

            string ns = type.Namespace;
            bool removeByNamespace = NamespacesToRemove.IsNotNullOrEmpty()
                                     &&
                                     NamespacesToRemove.Any(x => ns.Contains(x));
            if (!exclude && removeByNamespace)
                return true;

            return false;
        }

        public bool IsExcludeType(Type? type)
        {
            if (type is null)
                return false;

            bool excludeByType = typesToExclude.IsNotNullOrEmpty() 
                                 &&
                                 typesToExclude.Any(x => type.IsType(x));
            if (excludeByType)
                return true;

            string ns = type.Namespace;
            bool excludeByNamepsace = NamespacesToExclude.IsNotNullOrEmpty()
                                      &&
                                      NamespacesToExclude.Any(x => ns.Contains(x));

            if (excludeByNamepsace)
                return true;

            return false;
        }
    }
}
