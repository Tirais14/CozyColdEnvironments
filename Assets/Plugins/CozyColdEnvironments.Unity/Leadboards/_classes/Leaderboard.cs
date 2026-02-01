using ObservableCollections;

#nullable enable
namespace CCEnvs.Unity.Leaderboards
{
    public sealed class Leaderboard
        : 
        ILeaderboard
    {
        public ObservableDictionary<Identifier, ILeaderboardEntry> Profiles { get; } = new();

        public long GetScore(Identifier userProfileID)
        {
            if (!Profiles.TryGetValue(userProfileID, out var profile))
                throw new System.ArgumentException($"Cannot find user profile with id: {userProfileID}");

            return profile.Score;
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

        }
    }
}
