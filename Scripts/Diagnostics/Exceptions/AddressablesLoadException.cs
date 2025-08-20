#nullable enable
namespace UTIRLib.Diagnostics
{
    public class AddressablesLoadException : TirLibException
    {
        public AddressablesLoadException()
        {
        }

        public AddressablesLoadException(object assetKey)
            : 
            base(ConstructMessage(typeof(AddressablesLoadException), assetKey))
        {
        }
    }
}
