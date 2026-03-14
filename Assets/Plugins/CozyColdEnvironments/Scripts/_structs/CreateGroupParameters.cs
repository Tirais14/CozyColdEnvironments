using CommunityToolkit.Diagnostics;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Saves
{
    public readonly struct CreateGroupParameters : IEquatable<CreateGroupParameters>
    {
        public string GroupName { get; }

        public long SaveDataVersion { get; }

        public RedirectionMode Redirection { get; }

        public bool LoadOnFirstObjectRegistered { get; }

        public CreateGroupParameters(
            string groupName,
            long saveDataVersion = 0L,
            RedirectionMode redirection = default,
            bool loadOnFirstObjectRegistered = true
            )
        {
            Guard.IsNotNull(groupName, nameof(groupName));

            GroupName = groupName;
            SaveDataVersion = saveDataVersion;
            Redirection = redirection;
            LoadOnFirstObjectRegistered = loadOnFirstObjectRegistered;
        }

        public static implicit operator CreateGroupParameters(string groupName)
        {
            return new CreateGroupParameters(groupName);
        }

        public static bool operator ==(CreateGroupParameters left, CreateGroupParameters right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CreateGroupParameters left, CreateGroupParameters right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CreateGroupParameters WithGroupName(string groupName)
        {
            return new CreateGroupParameters(
                groupName,
                SaveDataVersion,
                Redirection,
                LoadOnFirstObjectRegistered
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CreateGroupParameters WithSaveDataVersion(long saveDataVersion)
        {
            return new CreateGroupParameters(
                GroupName,
                saveDataVersion,
                Redirection,
                LoadOnFirstObjectRegistered
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CreateGroupParameters WithRedirection(RedirectionMode redirection)
        {
            return new CreateGroupParameters(
                GroupName,
                SaveDataVersion,
                redirection,
                LoadOnFirstObjectRegistered
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CreateGroupParameters WithLoadOnFirstObjectRegistered(bool loadOnFirstObjectRegistered)
        {
            return new CreateGroupParameters(
                GroupName,
                SaveDataVersion,
                Redirection,
                loadOnFirstObjectRegistered
                );
        }

        public override bool Equals(object? obj)
        {
            return obj is CreateGroupParameters parameters && Equals(parameters);
        }

        public bool Equals(CreateGroupParameters other)
        {
            return GroupName == other.GroupName
                   &&
                   SaveDataVersion == other.SaveDataVersion 
                   &&
                   Redirection == other.Redirection
                   &&
                   LoadOnFirstObjectRegistered == other.LoadOnFirstObjectRegistered;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GroupName, SaveDataVersion, Redirection, LoadOnFirstObjectRegistered);
        }

        public override string ToString()
        {
            return $"({nameof(GroupName)}: {GroupName}; {nameof(SaveDataVersion)}: {SaveDataVersion}; {nameof(Redirection)}: {Redirection}; {nameof(LoadOnFirstObjectRegistered)}: {LoadOnFirstObjectRegistered})";
        }
    }
}
