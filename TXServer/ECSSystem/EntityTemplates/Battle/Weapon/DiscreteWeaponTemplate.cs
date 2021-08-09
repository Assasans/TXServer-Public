using System.Linq;
using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Weapon
{
    [SerialVersionUID(-1716200834009238305L)]
    public class DiscreteWeaponTemplate : WeaponTemplate
    {
        protected new static Entity CreateEntity(WeaponTemplate template, string configPath, Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = WeaponTemplate.CreateEntity(template, configPath, tank, battlePlayer);

            entity.Components.UnionWith(new Component[] {
                Config.GetComponent<ImpactComponent>(configPath),
                new DiscreteWeaponComponent(),
                Config.GetComponent<DamageWeakeningByDistanceComponent>(configPath, false),
                battlePlayer.TurretKickback == null
                    ? Config.GetComponent<KickbackComponent>(configPath)
                    : new KickbackComponent((float)battlePlayer.TurretKickback),
            }.Where(x => x != null));

            if (template.GetType() != typeof(ShaftBattleItemTemplate))
            {
                entity.AddComponent(battlePlayer.TurretUnloadEnergyPerShot == null
                    ? Config.GetComponent<DiscreteWeaponEnergyComponent>(configPath)
                    : new DiscreteWeaponEnergyComponent(1, (float) battlePlayer.TurretUnloadEnergyPerShot));
            }

            return entity;
        }
    }
}
