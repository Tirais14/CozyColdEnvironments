using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable
namespace UTIRLib
{
    public class AssetFailedLoadException : TirLibException
    {
        public AssetFailedLoadException()
        {
        }

        public AssetFailedLoadException(object key) : base($"Asset {key.ToString().WrapByDoubleQuotes()} load failed.")
        {
        }
    }
}
