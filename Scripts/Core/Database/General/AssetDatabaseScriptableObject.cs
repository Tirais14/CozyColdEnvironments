using UnityEngine;

#nullable enable

namespace UTIRLib.DB
{
    public class AssetDatabaseScriptableObject :
        AssetDatabase<AssetDatabaseItemScriptableObject, ScriptableObject>
    {
        public AssetDatabaseScriptableObject() : base()
        { }
    }
}