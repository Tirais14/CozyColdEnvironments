#nullable enable
using CCEnvs.Reflection.Data;
using System.Reflection;

namespace CCEnvs.Reflection
{
    public class ContextedMethodInfo : ContextedMemberInfo
    {
        public readonly MethodInfo value;

        public ContextedMethodInfo(object? context, MethodInfo method) : base(context)
        {
            value = method;
        }

        public object Invoke(params object[] arguments)
        {
            return value.Invoke(context, arguments);
        }

        public object Invoke(ExplicitArguments args)
        {
            return value.Invoke(context, (object?[])args);
        }
    }
}
