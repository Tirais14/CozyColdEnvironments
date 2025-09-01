using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs
{
    public static class ParameterInfoExtensions
    {
        public static ParameterModifier GetParameterModifiers(this ParameterInfo[] values)
        {
            if (values.IsEmpty())
                return default;

            var modifier = new ParameterModifier(values.Length);

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].ParameterType.IsByRef)
                    modifier[i] = true;
            }

            return modifier;
        }
    }
}
