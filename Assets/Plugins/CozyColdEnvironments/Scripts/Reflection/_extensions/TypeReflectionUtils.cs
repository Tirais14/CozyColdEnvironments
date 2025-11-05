using CCEnvs.FuncLanguage;
using System;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class TypeReflectionUtils
    {
        private static ReflectQuery imptQ => ReflectQuery.self.Reset()
                                                              .NonPublic()
                                                              .ByFullName()
                                                              .Name("op_Implicit");

        private static ReflectQuery exptQ => ReflectQuery.self.Reset()
                                                              .NonPublic()
                                                              .ByFullName()
                                                              .Name("op_Explicit");

        public static MethodInfo[] GetOverloadedCastOperators(this Type source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            var first = imptQ.From(source);
            var second = exptQ.From(source);

            return first.Methods()
                        .Concat(second.Methods())
                        .ToArray();
        }

        public static Maybe<MethodInfo> GetOverloadedCastOperator(this Type source,
            Type castType)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            CC.Guard.IsNotNull(castType, nameof(castType));

            var first = imptQ.From(source);
            var second = exptQ.From(source);

            return first.ExtraType(castType)
                        .Methods()
                        .Concat(second.ExtraType(castType).Methods())
                        .FirstOrDefault();
        }
    }
}
