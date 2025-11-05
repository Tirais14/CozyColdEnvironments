using CCEnvs.FuncLanguage;
using System;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class TypeReflectionUtils
    {


        private static Reflect imptQ => Reflect.self.Reset()
                                                    .Static()
                                                    .NonPublic()
                                                    .ByFullName()
                                                    .Name("op_Implicit");

        private static Reflect exptQ => Reflect.self.Reset()
                                                    .Static()
                                                    .NonPublic()
                                                    .ByFullName()
                                                    .Name("op_Explicit");

        public static MethodInfo[] GetOverloadedCastOperators(this Type source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            var t = source.Reflect()
                  .Static()
                  .NonPublic()
                  .ByFullName()
                  .Name("op_Implicit")
                  .Methods()
                  .Concat(source.Reflect()
                      .Static()
                      .NonPublic()
                      .ByFullName()
                      .Name("op_Explicit")
                      .Methods())
                  .ToArray();

            return t;
        }

        public static Maybe<MethodInfo> GetOverloadedCastOperator(this Type source,
            Type castType)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            CC.Guard.IsNotNull(castType, nameof(castType));

            return source.GetOverloadedCastOperators()
                .FirstOrDefault(x => x.ReturnType.IsType(castType));
        }
    }
}
