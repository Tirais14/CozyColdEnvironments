#nullable enable
using System;

namespace CCEnvs.Reflection
{
    public record TypeFinderParameters
    {
        public string AssemblyName { get; set; } = string.Empty;
        public string Namepsace { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public bool IgnoreCase { get; set; }
        public bool HasAssemblyName => AssemblyName.IsNotNullOrEmpty();
        public bool HasNamespace => Namepsace.IsNotNullOrEmpty();
        public bool HasTypeName => TypeName.IsNotNullOrEmpty();

        /// <exception cref="ArgumentNullException"></exception>
        public bool IsMatch(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            StringComparison comparison = IgnoreCase
                ? 
                StringComparison.InvariantCultureIgnoreCase
                : 
                StringComparison.InvariantCulture;

            bool result = true;
            if (HasAssemblyName)
                result = type.Assembly.GetName().Name.Contains(AssemblyName, comparison);

            if (!result)
                return false;

            if (HasNamespace && type.Namespace.IsNotNullOrEmpty())
                result = type.Namespace.Contains(Namepsace, comparison);

            if (!result)
                return false;

            if (HasTypeName)
                result = type.GetName().Contains(TypeName, comparison);

            return result;
        }
    }
}
