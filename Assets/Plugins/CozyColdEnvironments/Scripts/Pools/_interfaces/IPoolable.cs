#nullable enable
namespace CCEnvs.Pools
{
    public interface IPoolable
    {
        public void OnByPoolDespawned();

        public void OnByPoolSpawned();
    }
}
