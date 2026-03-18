#nullable enable
using CCEnvs.Attributes.Serialization;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;

namespace CCEnvs.Snapshots
{
    [Serializable, SerializationDescriptor("AnonymousSnapshot", "2aa9b2cb-9292-420f-ad60-ad002ba80efa")]
    public sealed record AnonymousSnapshot : Snapshot<object>
    {
        private readonly AnonymousSnapshotMember[] members;

        private readonly Type targetType;

        public IReadOnlyList<AnonymousSnapshotMember> Members => members;

        public override Type TargetType => targetType;

        public AnonymousSnapshot(Type targetType)
            :
            base(AnonymousSnapshotMemberResolver.ResolveMembers(targetType))
        {
            this.targetType = targetType;

            members ??= Array.Empty<AnonymousSnapshotMember>();
        }

        public AnonymousSnapshot(object target)
            :
            this(target?.GetType()!)
        {
            CC.Guard.IsNotNullTarget(target);

            CaptureFrom(target);
        }

        private AnonymousSnapshot(AnonymousSnapshotMember[] members)
        {
            CC.Guard.IsNotNull(members, nameof(members));

            targetType = TypeofCache<object>.Type;

            this.members = members;
        }

        protected override void OnCapture(object target)
        {
            base.OnCapture(target);

            for (int i = 0; i < members.Length; i++)
                members[i].CaptureValueFrom(target);
        }

        protected override void OnRestore(ref object target)
        {
            for (int i = 0; i < members.Length; i++)
                members[i].RestoreFromCaptured(target);
        }

        protected override void OnReset()
        {
            base.OnReset();

            for (int i = 0; i < members.Length; i++)
                members[i].ResetCapturedValue();
        }
    }
}
