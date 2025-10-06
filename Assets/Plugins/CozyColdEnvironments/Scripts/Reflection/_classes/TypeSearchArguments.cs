#nullable enable
using System;
using System.Linq;

namespace CCEnvs.Reflection
{
    public record TypeSearchArguments
    {
        public string AssemblyName { get; set; } = string.Empty;
        public string Namespace { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public bool IgnoreCase { get; set; }
        public bool HasAssemblyName => AssemblyName.IsNotNullOrEmpty();
        public bool HasNamespace => Namespace.IsNotNullOrEmpty();
        public bool HasTypeName => TypeName.IsNotNullOrEmpty();
        public Type[] DefinedAttributeTypes { get; set; } = Type.EmptyTypes;

        /// <exception cref="ArgumentNullException"></exception>
        public bool IsMatch(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            bool result = true;
            if (HasAssemblyName)
                result = type.Assembly.GetName().Name.ContainsOrdinal(AssemblyName, IgnoreCase);

            if (!result)
                return false;

            if (HasNamespace && type.Namespace.IsNotNullOrEmpty())
                result = type.Namespace.ContainsOrdinal(Namespace, IgnoreCase);

            if (!result)
                return false;

            if (HasTypeName)
                result = type.GetName().ContainsOrdinal(TypeName, IgnoreCase);

            if (DefinedAttributeTypes.IsNotEmpty()
                &&
                !DefinedAttributeTypes.All(x => type.IsDefined(x, inherit: true))
                )
                return false;

            return result;
        }
    }
}
