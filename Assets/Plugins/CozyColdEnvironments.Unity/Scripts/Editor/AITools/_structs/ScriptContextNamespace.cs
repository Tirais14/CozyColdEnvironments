#if UNITY_EDITOR
#nullable enable
using System;
using System.Runtime.Serialization;

namespace CCEnvs.UnityX.Editr.AITools
{
    [Serializable, DataContract]
    public class ScriptContextNamespace 
    {
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "onlyAPI")]
        public bool OnlyAPI { get; set; }
        [DataMember(Name = "includeDependencies")]
        public bool IncludeDependecies { get; set; }
        [DataMember(Name = "removeEmptyLines")]
        public bool RemoveEmptyLines { get; set; }
        [DataMember(Name = "dependeciesOnlyAPI")]
        public bool DependeciesOnlyAPI { get; set; }
        [DataMember(Name = "dependeciesRemoveEmptyLines")]
        public bool DependeciesRemoveEmptyLines { get; set; }

        [DataMember(Name = "matchSettings")]
        public StringMatchSettings MatchSettings { get; set; }
    }
}
#endif
