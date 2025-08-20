using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable
namespace UTIRLib
{
    public class AddressablesLoadException : TirLibException
    {
        public AddressablesLoadException()
        {
        }

        public AddressablesLoadException(object key)
            : 
            base($"Asset {key.ToString().WrapByDoubleQuotes()} load failed.")
        {
        }
    }
}
