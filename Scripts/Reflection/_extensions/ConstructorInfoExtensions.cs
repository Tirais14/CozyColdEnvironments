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
    }
}
