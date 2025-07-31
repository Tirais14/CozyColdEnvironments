using System.Reflection;

#nullable enable
namespace UTIRLib
{
    public static class ParameterInfoExtensions
    {
        public static ParameterModifier GetParameterModifier(this ParameterInfo[] values)
        {
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
