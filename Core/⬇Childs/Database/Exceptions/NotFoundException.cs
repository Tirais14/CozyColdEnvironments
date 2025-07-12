using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

namespace UTIRLib.DB
{
    public sealed class NotFoundException : TirLibException
    {
        private const string MESSAGE = "Not found{0} in database registry.";

        public NotFoundException() : base(MESSAGE, " item")
        { }

        public NotFoundException(object value) : base(MESSAGE, $" value {value.GetTypeName()}")
        { }
    }
}