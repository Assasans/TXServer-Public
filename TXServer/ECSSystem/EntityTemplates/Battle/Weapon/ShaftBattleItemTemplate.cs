using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Weapon
{
    [SerialVersionUID(-2537616944465628484L)]
    public class ShaftBattleItemTemplate : DiscreteWeaponTemplate
    {
        private const string GarageWeaponConfig = "garage/weapon/shaft";

        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new ShaftBattleItemTemplate(), GarageWeaponConfig, tank, battlePlayer);
            entity.Components.UnionWith(new Component[]
            {
                new ShaftComponent(),
                Config.GetComponent<ShaftAimingImpactComponent>(GarageWeaponConfig),
                Config.GetComponent<ShaftAimingSpeedComponent>(GarageWeaponConfig),
                new ShaftEnergyComponent(0.2857f, 1, 0.2f, 0.143f),
                Config.GetComponent<ShaftStateConfigComponent>("battle/weapon/shaft")
            });

            return entity;
        }
    }
}
