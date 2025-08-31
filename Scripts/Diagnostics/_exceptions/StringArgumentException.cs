#nullable enable
using static CCEnvs.Diagnostics.ExceptionMessageConstructor;

namespace CCEnvs.Diagnostics
{
    public sealed class StringArgumentException : CCArgumentException
    {
        public StringArgumentException()
        {
        }

        public StringArgumentException(string paramName)
            :
            base(ConstructMessage<StringArgumentException>(paramName))
        {
        }

        public StringArgumentException(string paramName, string? value)
            : 
            base($"String: {StringException.Resolve(value)}. {ConstructMessage<StringArgumentException>(paramName)}")
        {
        }
    }
}