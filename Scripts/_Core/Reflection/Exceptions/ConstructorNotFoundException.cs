using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection.Diagnostics
{
    public class ConstructorNotFoundException : TirLibException
    {
        public ConstructorNotFoundException()
        {
        }

        public ConstructorNotFoundException(TypeInstanceFactory.Parameters parameters)
            : base($"Constructor not found. Parameters: {parameters}")
        {
        }
    }
}
