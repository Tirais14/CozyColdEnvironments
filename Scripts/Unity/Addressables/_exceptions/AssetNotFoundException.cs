using CCEnvs.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public class AssetNotFoundException : CCException
    {
        public AssetNotFoundException()
        {
        }

        public AssetNotFoundException(string assetName, bool ignoreCase, Exception? innerException = null)
            : 
            base($"{nameof(assetName)}: {assetName} | {nameof(ignoreCase)}: {ignoreCase}.", innerException)
        {
        }

        public AssetNotFoundException(object assetID, Exception? innerException = null)
            :
            base($"{nameof(assetID)}: {assetID}.", innerException)
        {
        }

        protected AssetNotFoundException(string message, Exception? innerException = null)
            :
            base(message, innerException)
        {
        }

        public static AssetNotFoundException ByMessage(
            string message,
            Exception? innerException = null)
        {
            return new AssetNotFoundException(message, innerException);
        }
    }
}
