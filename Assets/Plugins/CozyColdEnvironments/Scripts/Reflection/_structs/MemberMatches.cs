using System.Collections.ObjectModel;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public readonly struct MemberMatches
    {
        public readonly int leftProcessedMemberCount;
        public readonly int righProcessedMemberCount;
        public readonly ReadOnlyCollection<MemberInfo> values;

        public MemberMatches(int leftProcessedMemberCount,
                             int righProcessedMemberCount,
                             ReadOnlyCollection<MemberInfo> matches)
        {
            this.leftProcessedMemberCount = leftProcessedMemberCount;
            this.righProcessedMemberCount = righProcessedMemberCount;
            this.values = matches;
        }
    }
}
