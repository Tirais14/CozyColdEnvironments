using CCEnvs.Reflection.Data;
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

        /// <exception cref="System.ArgumentNullException"></exception>
        public static CCParameters AsCCParameters(this ParameterInfo[] values)
        {
            if (values is null)
                throw new System.ArgumentNullException(nameof(values));

            return new CCParameters(values.Select(x => x.AsCCParameterInfo()).ToArray());
        }

        /// <exception cref="System.ArgumentNullException"></exception>
        public static CCParameterInfo AsCCParameterInfo(this ParameterInfo value)
        {
            if (value is null)
                throw new System.ArgumentNullException(nameof(value));

            return new CCParameterInfo(value.ParameterType,
                value.HasDefaultValue,
                value.GetRequiredCustomModifiers().Concat(value.GetOptionalCustomModifiers()).ToArray());
        }
    }
}
