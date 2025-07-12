using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable
namespace UTIRLib
{
    public class TypeNotFoundException : TirLibException
    {
        public TypeNotFoundException()
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
