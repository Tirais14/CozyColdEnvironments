#nullable enable
using CCEnvs.Attributes.Serialization;
using CCEnvs.Json;
using CCEnvs.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCEnvs.Snapshots
{
    [Serializable, SerializationDescriptor("AnonymousSnapshot", "2aa9b2cb-9292-420f-ad60-ad002ba80efa")]
    public sealed record AnonymousSnapshot : Snapshot<object>
    {
        [JsonProperty("members")]
        private AnonymousSnapshotMember[] members;

        [JsonIgnore]
        public IReadOnlyList<AnonymousSnapshotMember> Members => members;

        public override Type TargetType => TypeofCache<object>.Type;

        public AnonymousSnapshot(Type targetType)
            :
            this(AnonymousSnapshotMemberResolver.ResolveMembers(targetType))
        {
            members ??= Array.Empty<AnonymousSnapshotMember>();
        }

        public AnonymousSnapshot(IEnumerable<AnonymousSnapshotMember> members)
            :
            this(members.ToArray())
        {
        }

        //[JsonConstructor]
        internal AnonymousSnapshot(AnonymousSnapshotMember[] members)
        {
            CC.Guard.IsNotNull(members, nameof(members));

            this.members = members;
        }

        internal AnonymousSnapshot(object target)
            :
            this(target?.GetType()!)
        {
            CC.Guard.IsNotNullTarget(target);

            CaptureFrom(target);
        }

        private AnonymousSnapshot()
            :
            this(Array.Empty<AnonymousSnapshotMember>())
        {

        }

        protected override void OnCapture(object target)
        {
            base.OnCapture(target);

            if (members is null)
                return;

            for (int i = 0; i < members.Length; i++)
                members[i].CaptureValueFrom(target);
        }

        protected override void OnRestore(ref object target)
        {
            if (members is null)
                return;

            for (int i = 0; i < members.Length; i++)
                members[i].RestoreFromCaptured(target);
        }

        protected override void OnReset()
        {
            base.OnReset();

            if (members is null)
                return;

            for (int i = 0; i < members.Length; i++)
                members[i].ResetCapturedValue();
        }
    }
}
