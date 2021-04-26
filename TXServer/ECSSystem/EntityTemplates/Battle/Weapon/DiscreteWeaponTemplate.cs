using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-1716200834009238305L)]
    public class DiscreteWeaponTemplate : WeaponTemplate
    {
        protected static new Entity CreateEntity(WeaponTemplate template, string configPath, Entity tank, BattlePlayer battlePlayer)
        {
            Entity entity = WeaponTemplate.CreateEntity(template, configPath, tank, battlePlayer);
            // todo: read from configs
            entity.Components.Add(new ImpactComponent(2.5f));
            entity.Components.Add(new DiscreteWeaponComponent());
            entity.Components.Add(new DamageWeakeningByDistanceComponent(52.174f, 83.091f, 128.261f));

            entity.Components.Add(battlePlayer.TurretKickback == null
                ? new KickbackComponent(3f)
                : new KickbackComponent((float) battlePlayer.TurretKickback));

            entity.Components.Add(battlePlayer.TurretUnloadEnergyPerShot == null
                ? new DiscreteWeaponEnergyComponent(1.0f, 1.0f)
                : new DiscreteWeaponEnergyComponent(1.0f, (float) battlePlayer.TurretUnloadEnergyPerShot));

            return entity;
        }
    }
}