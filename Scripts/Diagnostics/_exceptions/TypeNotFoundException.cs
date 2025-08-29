using CozyColdEnvironments.Extensions;
using CozyColdEnvironments.Reflection;

#nullable enable
namespace CozyColdEnvironments.Diagnostics
{
    public class TypeNotFoundException : TirLibException
    {
        public TypeNotFoundException()
        {
        }

        public TypeNotFoundException(TypeSearchingParameters parameters) 
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
