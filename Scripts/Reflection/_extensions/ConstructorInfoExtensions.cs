using System.Linq;
using System.Reflection;
using UTIRLib.Reflection.ObjectModel;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class ConstructorInfoExtensions
    {
        public static Signature GetSignature(this ConstructorInfo constructor)
        {
            return new Signature(constructor.GetParameters().Select(x => x.ParameterType));
        }

        /// <exception cref="System.ArgumentNullException"></exception>
        public static ParameterModifier GetParameterModifiers(this ConstructorInfo value)
        {
            if (value is null)
                throw new System.ArgumentNullException(nameof(value));

            ParameterInfo[] parameters = value.GetParameters();
            var modifiers = new ParameterModifier(parameters.Length);
            ParameterInfo parameter;
            for (int i = 0; i < parameters.Length; i++)
            {
                parameter = parameters[i];
                if (parameter.GetOptionalCustomModifiers().IsNotEmpty()
                    ||
                    parameter.GetRequiredCustomModifiers().IsNotEmpty()
                    )
                    modifiers[i] = true;
            }
            return modifiers;   
        }
    }
}
