#if UNITY_EDITOR
#nullable enable
using CCEnvs.Attributes.Serialization;
using CCEnvs.Serialization;
using System;
using System.Runtime.Serialization;

namespace CCEnvs.UnityX.Editr.AITools
{
    [Serializable, DataContract]
    [PolymorphSerializable, SerializationDescriptor("b1234567-8c9d-4e0f-a1b2-3c4d5e6f7a8b")] // Сгенерируйте новый GUID
    public sealed class ScriptContextScriptInfo
    {
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "matchSettings")]
        public StringMatchSettings MatchSettings { get; set; }

        [DataMember(Name = "onlyAPI")]
        public bool OnlyAPI { get; set; }

        [DataMember(Name = "includeDependencies")]
        public bool IncludeDependecies { get; set; }

        [DataMember(Name = "removeEmptyLines")]
        public bool RemoveEmptyLines { get; set; }

        [DataMember(Name = "dependenciesOnlyAPI")]
        public bool DependeciesOnlyAPI { get; set; }

        [DataMember(Name = "dependenciesRemoveEmptyLines")]
        public bool DependeciesRemoveEmptyLines { get; set; }
    }
}
#endif