using CCEnvs.FuncLanguage;
using CCEnvs.Json.Converters;
using CCEnvs.Reflection;
using System;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    [JsonConverter(typeof(SnapshotConverter))]
    public abstract class Snapshot
    {
  //      [JsonInclude]
		//[JsonPropertyName("$type")]
  //      public string SelfTypeReference { get; private set; }

        public Snapshot()
        {
            //SelfTypeReference = GetType().GetTypeReference();
        }
    }

    [Serializable]
    [JsonConverter(typeof(SnapshotConverter))]
    public abstract class Snapshot<T> : Snapshot, ISnapshot<T>
    {
#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        [JsonIgnore]
        public Maybe<T> Target { get; private set; }

        public Snapshot()
        {
        }

        public Snapshot(T target)
            :
            this()
        {
            CC.Guard.IsNotNull(target, nameof(target));

            Target = target;
        }

        public T Restore() => Restore(Target.Raw);

        public abstract T Restore(T? target);
    }
}
