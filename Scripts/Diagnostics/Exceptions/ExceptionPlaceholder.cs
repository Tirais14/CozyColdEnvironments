using System;

#nullable enable
namespace UTIRLib.Diagnostics
{
    [Obsolete("Use LogicException instead.")]
    public class ExceptionPlaceholder : TirLibException
    {
        public ExceptionPlaceholder()
        {
        }

        public ExceptionPlaceholder(string message) : base(message)
        {
        }

        public ExceptionPlaceholder(string notFormattedMessage,
                                    params object[] args) : base(notFormattedMessage,
                                                                 args)
        {
        }
    }
}
