using System.Linq;
using System.Reflection;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class ConstructorInfoExtensions
    {
        public static InvokableSignature GetSignature(this ConstructorInfo constructor)
        {
            return new InvokableSignature(constructor.GetParameters().Select(x => x.ParameterType));
        }
    }
}
