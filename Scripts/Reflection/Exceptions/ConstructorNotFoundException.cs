using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection.Diagnostics
{
    public class ConstructorNotFoundException : TirLibException
    {
        public ConstructorNotFoundException()
        {
        }

        public ConstructorNotFoundException(TypeMemberParameters parameters)
            : base($"Constructor not found. Parameters: {parameters}")
        {
        }
    }
}
