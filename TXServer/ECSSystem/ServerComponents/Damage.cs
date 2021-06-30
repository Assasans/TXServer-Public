using TXServer.ECSSystem.Components.Battle;

namespace TXServer.ECSSystem.ServerComponents
{
    public static class Damage
    {
        public class DamagePerPelletPropertyComponent : RangedComponent
        {
        }

        public class DamagePerSecondPropertyComponent : RangedComponent
        {
        }

        public class MinDamagePropertyComponent : RangedComponent
        {
        }
        public class MaxDamagePropertyComponent : RangedComponent
        {
        }

        public class WeaponCooldown : RangedComponent, IConvertibleComponent<WeaponCooldownComponent>
        {
            public void Convert(WeaponCooldownComponent component) => component.CooldownIntervalSec = FinalValue;
        }
    }
}
