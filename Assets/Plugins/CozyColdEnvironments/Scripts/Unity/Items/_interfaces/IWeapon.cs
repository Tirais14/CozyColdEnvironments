#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IWeapon : IItem
    {
        float DamageValue { get; }
    }
}
