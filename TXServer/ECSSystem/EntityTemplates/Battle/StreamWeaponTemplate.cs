using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1430285569243L)]
    public class StreamWeaponTemplate : WeaponTemplate
    {
        protected static new Entity CreateEntity(WeaponTemplate template, string configPath, Entity tank)
        {
            Entity entity = WeaponTemplate.CreateEntity(template, configPath, tank);
            entity.Components.Add(new StreamWeaponComponent());
            entity.Components.Add(new StreamWeaponEnergyComponent(.167f, .25f));

            return entity;
        }
    }
}