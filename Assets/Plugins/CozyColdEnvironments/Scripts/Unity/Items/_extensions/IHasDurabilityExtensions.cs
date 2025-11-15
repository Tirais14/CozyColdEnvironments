using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public static class IHasDurabilityExtensions
    {
        public static float DecreaseDurabilityBy(this IHasDurability source,
            IWeapon weapon)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            CC.Guard.IsNotNull(weapon, nameof(weapon));

            return source.DecreaseDurability(weapon.DamageValue);
        }
    }
}
