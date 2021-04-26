using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-2537616944465628484L)]
    public class ShaftBattleItemTemplate : DiscreteWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank, BattlePlayer battlePlayer)
        {
            Entity entity = CreateEntity(new ShaftBattleItemTemplate(), "battle/weapon/shaft", tank, battlePlayer);
            entity.Components.Add(new ShaftComponent());
            entity.Components.Add(new ShaftStateConfigComponent(1, 1, 1));
            entity.Components.Add(new ShaftAimingImpactComponent(1));
            entity.Components.Add(new ShaftEnergyComponent(1, 1, 1, 1));
            entity.Components.Add(new ShaftAimingSpeedComponent(1, 1, 1, 1));
            return entity;
        }
    }
}