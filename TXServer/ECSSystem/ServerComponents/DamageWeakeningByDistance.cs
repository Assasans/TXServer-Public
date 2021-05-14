using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.ServerComponents
{
    public static class DamageWeakeningByDistance
    {
        public class MinDamagePercentPropertyComponent : RangedComponent, IConvertibleComponent<DamageWeakeningByDistanceComponent>
        {
            public void Convert(DamageWeakeningByDistanceComponent component) => component.MinDamagePercent = FinalValue;
        }
        public class MinDamageDistancePropertyComponent : RangedComponent, IConvertibleComponent<DamageWeakeningByDistanceComponent>
        {
            public void Convert(DamageWeakeningByDistanceComponent component) => component.RadiusOfMinDamage = FinalValue;
        }
        public class MaxDamageDistancePropertyComponent : RangedComponent, IConvertibleComponent<DamageWeakeningByDistanceComponent>
        {
            public void Convert(DamageWeakeningByDistanceComponent component) => component.RadiusOfMaxDamage = FinalValue;
        }
    }
}
