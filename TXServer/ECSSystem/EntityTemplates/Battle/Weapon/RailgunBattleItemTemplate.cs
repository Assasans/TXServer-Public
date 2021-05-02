using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-6419489500262573655L)]
    public class RailgunBattleItemTemplate : DiscreteWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new RailgunBattleItemTemplate(), "battle/weapon/railgun", tank, battlePlayer);
            entity.Components.Add(new RailgunComponent());
            
            entity.Components.Add(battlePlayer.TurretUnloadEnergyPerShot == null
                ? new RailgunChargingWeaponComponent(1f)
                : new RailgunChargingWeaponComponent((float) battlePlayer.TurretUnloadEnergyPerShot));

            return entity;
        }
    }
}