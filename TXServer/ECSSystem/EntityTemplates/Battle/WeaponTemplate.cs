using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Energy;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1430285417001)]
    public class WeaponTemplate : IEntityTemplate
    {
        // Instead of getting template & configPath as parameters, it better should get market/user item entity and convert it to battle item.
        public static Entity CreateEntity(WeaponTemplate template, string configPath, Entity tank)
        {
            return new Entity(new TemplateAccessor(template, configPath.Replace("garage", "battle")),
                tank.GetComponent<TankPartComponent>(),
                new WeaponComponent(),
                new WeaponCooldownComponent(1.3f),
                new WeaponRotationComponent(113.667f, 113.333f, 113.333f),
                new WeaponEnergyComponent(1.0f),
                tank.GetComponent<UserGroupComponent>(),
                tank.GetComponent<TankGroupComponent>(),
                tank.GetComponent<BattleGroupComponent>());
        }
    }
}