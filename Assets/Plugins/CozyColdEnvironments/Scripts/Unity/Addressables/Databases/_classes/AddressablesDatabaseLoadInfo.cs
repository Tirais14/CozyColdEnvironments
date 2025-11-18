using CCEnvs.FuncLanguage;
using CCEnvs.Unity.EditorSerialization;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    [CreateAssetMenu(fileName = nameof(AddressablesDatabaseLoadInfo),
        menuName = nameof(CCEnvs) + "/Addressables/Databases/Load Info")]
    public class AddressablesDatabaseLoadInfo : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Often implied category. If null or empty will be used this.name without \"LoadInfo\" suffix if exists.")]
        private Maybe<Identifier> ID;

        [SerializeField]
        private SerializedType databaseType = new(false);

        [field: SerializeField]
        public SerializedType AssetType { get; private set; } = null!;

        [field: SerializeField]
        public AssetLabels AssetLabels { get; private set; }

        public AssetDatabaseKey AssetDatabaseKey {
            get => ID.BiMap(
                some: id => new AssetDatabaseKey(AssetType, id),
                none: () => new AssetDatabaseKey(AssetType, new Identifier() { Text = name })
                ).Raw;
        }

        public Type GetDatabaseType()
        {
            if (databaseType.Value is not null)
                return databaseType.Value;

            Type resolved = typeof(AddressablesDatabase<>).MakeGenericType(AssetType);

            return resolved;
        }
    }
}
