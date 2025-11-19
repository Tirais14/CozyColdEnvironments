#nullable enable
using CCEnvs.FuncLanguage;
using System;
using System.Linq;

namespace CCEnvs.Reflection
{
    public record TypeSearchArguments
    {
        public Maybe<string> Assembly { get; set; } = string.Empty;
        public Maybe<string> Namespace { get; set; } = string.Empty;
        public Maybe<string> TypeName { get; set; } = string.Empty;
        public bool IgnoreCase { get; set; }
        public Type[] DefinedAttributeTypes { get; set; } = Type.EmptyTypes;

        /// <exception cref="ArgumentNullException"></exception>
        public bool IsMatch(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            bool result = true;
            if (Assembly.IsSome)
                result = type.Assembly.GetName().Name.ContainsOrdinal(Assembly.GetValueUnsafe(), IgnoreCase);

            if (!result)
                return false;

            if (Namespace.IsSome && type.Namespace.IsNotNullOrEmpty())
                result = type.Namespace.ContainsOrdinal(Namespace.GetValueUnsafe(), IgnoreCase);

            if (!result)
                return false;

            if (TypeName.IsSome)
                result = type.GetName().ContainsOrdinal(TypeName.GetValueUnsafe(), IgnoreCase);

            if (DefinedAttributeTypes.IsNotEmpty()
                &&
                !DefinedAttributeTypes.All(x => type.IsDefined(x, inherit: true))
                )
                return false;

            return result;
        }
    }
}
