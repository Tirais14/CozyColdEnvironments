#nullable enable
using CCEnvs.Reflection.Data;
using System;
using System.Reflection;

namespace CCEnvs.Diagnostics
{
    public class ConstructorNotFoundException : MethodNotFoundException
    {
        public ConstructorNotFoundException()
        {
        }

        public ConstructorNotFoundException(Type reflectedType)
            :
            base(reflectedType)
        {
        }

        public ConstructorNotFoundException(Type reflectedType, BindingFlags bindings)
            : 
            base(reflectedType, ".ctor", bindings)
        {
        }

        public ConstructorNotFoundException(Type reflectedType,
                                            BindingFlags bindings,
                                            Signature signature)
            :
            base(reflectedType,
                 ".ctor",
                 bindings,
                 signature)
        {
        }

        public ConstructorNotFoundException(Type reflectedType,
                                            BindingFlags bindings,
                                            Signature signature,
                                            Signature genericParams)
            :
            base(reflectedType,
                 ".ctor",
                 bindings,
                 signature,
                 genericParams)
        {
        }
    }
}
