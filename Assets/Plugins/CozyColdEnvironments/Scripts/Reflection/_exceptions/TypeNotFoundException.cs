using CCEnvs.Diagnostics;
using CCEnvs.Extensions;

#nullable enable
namespace CCEnvs.Reflection
{
    public class TypeNotFoundException : CCException
    {
        public TypeNotFoundException()
        {
        }

        public TypeNotFoundException(TypeSearchArguments parameters) 
            : base($"Parameters: {parameters}.")
        {
        }

        public TypeNotFoundException(string typeName) : base($"Search name: {typeName.WrapByDoubleQuotes()}")
        {
        }

        public TypeNotFoundException(string typeName, string message)
            : base($"Search name: {typeName.WrapByDoubleQuotes()}. {message}")
        {
        }
    }
}
