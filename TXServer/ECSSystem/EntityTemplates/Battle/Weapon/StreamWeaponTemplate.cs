using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Weapon
{
    [SerialVersionUID(1430285569243L)]
    public class StreamWeaponTemplate : WeaponTemplate
    {
        protected new static Entity CreateEntity(WeaponTemplate template, string configPath, Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = WeaponTemplate.CreateEntity(template, configPath, tank, battlePlayer);
            entity.Components.Add(new StreamWeaponComponent());

            StreamWeaponEnergyComponent energyComponent = Config.GetComponent<StreamWeaponEnergyComponent>(configPath);
            if (battlePlayer.TurretUnloadEnergyPerShot != null)
                energyComponent.UnloadEnergyPerSec = (float) battlePlayer.TurretUnloadEnergyPerShot;

            entity.AddComponent(energyComponent);

            return entity;
        }
    }
}
