using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace UTIRLib.Reflection
{
    public record ConstructorParameters : TypeMemberParameters
    {
        public KeyValuePair<Type, object?>[] ArgumentsData {
            get
            {
                if (Signature.IsNullOrEmpty())
                    return Array.Empty<KeyValuePair<Type, object?>>();

                var temp = new KeyValuePair<Type, object?>[Signature.Length];

                for (int i = 0; i < temp.Length; i++)
                    temp[i] = new KeyValuePair<Type, object?>(Signature[i], Arguments[i]);

                return temp;
            }
            set
            {
                Signature = value.Select(x => x.Key).ToArray();
                Arguments = value.Select(x => x.Value).ToArray();
            }
        }

        public Type[] Signature { get; private set; } = Array.Empty<Type>();
        public object?[] Arguments { get; private set; } = Array.Empty<object?>();
    }
}
