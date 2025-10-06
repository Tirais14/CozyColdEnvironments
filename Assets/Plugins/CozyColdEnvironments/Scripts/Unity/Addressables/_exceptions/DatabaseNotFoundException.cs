using CCEnvs.Diagnostics;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public class DatabaseNotFoundException : CCException
    {
        public DatabaseNotFoundException()
        {
        }

        public DatabaseNotFoundException(Type dbAssetType, Exception? innerException = null)
            :
            base($"{nameof(dbAssetType)}: {dbAssetType}", innerException)
        {
        }

        protected DatabaseNotFoundException(string message, Exception? innerException = null)
            :
            base(message, innerException)
        {
        }
    }
}
