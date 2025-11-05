using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class ConstructorInfoExtensions
    {
        /// <exception cref="System.ArgumentNullException"></exception>
        public static ParameterModifier GetParameterModifiers(this ConstructorInfo value)
        {
            if (value is null)
                throw new System.ArgumentNullException(nameof(value));

            return value.GetParameters().GetParameterModifiers();
        }
    }
}
