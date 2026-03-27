#nullable enable
using CCEnvs.Attributes.Serialization;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

#pragma warning disable IDE0044
namespace CCEnvs.Snapshots
{
    [JsonObject(MemberSerialization.OptIn), DataContract]
    [Serializable, SerializationDescriptor("AnonymousSnapshot", "2aa9b2cb-9292-420f-ad60-ad002ba80efa")]
    public sealed record AnonymousSnapshot : Snapshot<object>, IEnumerable<AnonymousSnapshotMember>
    {
        private readonly Lazy<Dictionary<string, int>> memberIdxByNames = new(() => new Dictionary<string, int>(2));

        [JsonProperty("members")]
        private List<AnonymousSnapshotMember> members = new();

        public AnonymousSnapshotMember? this[string name] => FindMemberByName(name, out _);

        public IReadOnlyList<AnonymousSnapshotMember> Members => members;

        public override Type TargetType => TypeofCache<object>.Type;

        public object MembersGate { get; } = new();

        public bool IgnoreNullOrRestore { get; set; } = true;

        public AnonymousSnapshot(Type targetType)
            :
            this(AnonymousSnapshotMemberResolver.ResolveMembers(targetType))
        {
        }

        public AnonymousSnapshot(IEnumerable<AnonymousSnapshotMember> members)
        {
            CC.Guard.IsNotNull(members, nameof(members));

            this.members.AddRange(members);
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

        public AnonymousSnapshot SetIgnoreNullOnRestore(bool value)
        {
            IgnoreNullOrRestore = value;
            return this;
        }

        public bool TryAddMember(AnonymousSnapshotMember member)
        {
            Guard.IsNotNull(member);

            lock (MembersGate)
            {
                if (members.Contains(member))
                    return false;

                members.Add(member);
            }
            return true;
        }

        public bool RemoveMemberByName(
            string name,
            [NotNullWhen(true)] out AnonymousSnapshotMember? removed
            )
        {
            Guard.IsNotNull(name);

            if (FindMemberByName(name, out var idx).IsNull(out removed))
                return false;

            lock (MembersGate)
                members.RemoveAt(idx);

            return true;
        }

        public AnonymousSnapshotMember? FindMemberByName(string name, out int idx)
        {
            Guard.IsNotNull(name);

            lock (MembersGate)
            {
                if (this.memberIdxByNames.TryGetValue(out var memberIdxByNames)
                    &&
                    memberIdxByNames.TryGetValue(name, out idx))
                {
                    if (idx >= members.Count)
                        return null;

                    return members[idx];
                }

                for (int i = 0; i < members.Count; i++)
                {
                    if (members[i].MemberInfo.Name.StartsWith(name))
                    {
                        idx = i;
                        this.memberIdxByNames.Value.Add(name, i);
                        return members[i];
                    }
                }
            }

            idx = -1;
            return null;
        }

        public IEnumerator<AnonymousSnapshotMember> GetEnumerator()
        {
            lock (MembersGate)
                for (int i = 0; i < members.Count; i++)
                    yield return members[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected override void OnCapture(object target)
        {
            base.OnCapture(target);

            if (members is null)
                return;

            lock (MembersGate)
                for (int i = 0; i < members.Count; i++)
                    members[i].CaptureValueFrom(target);
        }

        protected override void OnRestore(ref object target)
        {
            if (members is null)
                return;

            lock (MembersGate)
                for (int i = 0; i < members.Count; i++)
                    members[i].RestoreFromCaptured(target);
        }

        protected override void OnReset()
        {
            base.OnReset();

            if (members is null)
                return;

            lock (MembersGate)
                for (int i = 0; i < members.Count; i++)
                    members[i].ResetCapturedValue();
        }
    }
}
