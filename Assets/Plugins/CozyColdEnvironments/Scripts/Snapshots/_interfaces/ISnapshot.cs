#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CCEnvs.Snapshots
{
    [JsonConverter(typeof(PolymorphJsonConverter<ISnapshot>))]
    public interface ISnapshot
    {
        [JsonIgnore]
        Maybe<object> Target { get; set; }

        [JsonIgnore]
        Type TargetType { get; }

        public bool CanRestoreWithoutTarget { get; }
        public bool IgnoreTarget { get; }

        Maybe<object> Restore();
        Maybe<object> Restore(object? target);

        bool CanRestore();
    }

    public interface ISnapshot<T> : ISnapshot
    {
        [JsonIgnore]
        new Maybe<T> Target { get; set; }

        [JsonIgnore]
        Maybe<object> ISnapshot.Target {
            get => Target;
            set => Target = value.Cast<T>().RightTarget;
        }

        new Maybe<T> Restore();
        Maybe<T> Restore(T? target);

        Maybe<object> ISnapshot.Restore() => Restore();
        Maybe<object> ISnapshot.Restore(object? target) => Restore(target.To<T>());
    }

    public static class ISnapshotExtensions
    {
        public static void RestoreSnapshotStates<T>(this IEnumerable<T> states)
            where T : struct, ISnapshot
        {
            CC.Guard.IsNotNull(states, nameof(states));

            foreach (var state in states)
                state.Restore();
        }

        public static void RestoreSnapshotStates(this IEnumerable<ISnapshot> states)
        {
            CC.Guard.IsNotNull(states, nameof(states));

            foreach (var state in states)
                state.Restore();
        }
    }
}
