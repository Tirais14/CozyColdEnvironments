using CCEnvs.Extensions;
using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public class TypeNotFoundException : CCException
    {
        public TypeNotFoundException()
        {
        }

        public TypeNotFoundException(TypeFinderParameters parameters) 
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
