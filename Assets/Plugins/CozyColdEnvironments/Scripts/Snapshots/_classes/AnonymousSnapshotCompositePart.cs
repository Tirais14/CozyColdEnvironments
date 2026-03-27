using CCEnvs.Attributes.Serialization;
using CCEnvs.TypeMatching;
using Newtonsoft.Json;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable, SerializationDescriptor("AnonymousSnapshotCompositePart", "0b98065c-b512-4f01-98b9-720e5eae7f91")]
    public class AnonymousSnapshotCompositePart : AnonymousSnapshotMember
    {
        [JsonProperty("snapshot")]
        public ISnapshot Snapshot { get; private set; }

        public AnonymousSnapshotCompositePart(
            FieldInfo fieldInfo,
            ISnapshot snapshot
            )
            :
            base(fieldInfo)
        {
            CC.Guard.IsNotNull(snapshot, nameof(snapshot)); 

            Snapshot = snapshot;
        }

        public AnonymousSnapshotCompositePart(
            PropertyInfo propInfo,
            ISnapshot snapshot
            )
            :
            base(propInfo)
        {
            CC.Guard.IsNotNull(snapshot, nameof(snapshot));

            Snapshot = snapshot;
        }

#nullable disable warnings
        private AnonymousSnapshotCompositePart()
        {
        }
#nullable enable warnings

        public override void ResetCapturedValue() => Snapshot.Reset();

        public override void CaptureValueFrom(object target)
        {
            if (GetValue(target).IsNull(out var value))
            {
                Snapshot.Reset();
                return;
            }

            Snapshot.CaptureFrom(value);
        }

        public override void RestoreFromCaptured(object target)
        {
            var value = GetValue(target);

            if (Snapshot.TryRestore(value, out value))
            {
                this.PrintException(new InvalidOperationException($"Cannot restore value. Snapshot: {Snapshot}; value: {value}"));
                return;
            }

            SetValue(target, value);
        }

        internal void SetUnderlyingSnapshot(ISnapshot snapshot)
        {
            CC.Guard.IsNotNull(snapshot, nameof(snapshot));

            Snapshot = snapshot;
        }
    }
}
