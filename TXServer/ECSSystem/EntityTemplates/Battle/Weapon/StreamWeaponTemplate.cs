using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1430285569243L)]
    public class StreamWeaponTemplate : WeaponTemplate
    {
        protected new static Entity CreateEntity(WeaponTemplate template, string configPath, Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = WeaponTemplate.CreateEntity(template, configPath, tank, battlePlayer);
            entity.Components.Add(new StreamWeaponComponent());

            entity.Components.Add(battlePlayer.TurretUnloadEnergyPerShot == null
                ? new StreamWeaponEnergyComponent(.167f, .25f)
                : new StreamWeaponEnergyComponent(.167f, (float) battlePlayer.TurretUnloadEnergyPerShot));

            return entity;
        }
    }
}
