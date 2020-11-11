using System;
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
        public static Entity CreateEntity(Entity userItem, Entity tank)
        {
            // TODO: Select template based on given item template
            switch (userItem.TemplateAccessor.Template)
            {
                case SmokyUserItemTemplate _:
                    return SmokyBattleItemTemplate.CreateEntity(tank);
                case FlamethrowerUserItemTemplate _:
                    return FlamethrowerBattleItemTemplate.CreateEntity(tank);
                case FreezeUserItemTemplate _:
                    return FreezeBattleItemTemplate.CreateEntity(tank);
                case IsisUserItemTemplate _:
                    return IsisBattleItemTemplate.CreateEntity(tank);
                case ThunderUserItemTemplate _:
                    return ThunderBattleItemTemplate.CreateEntity(tank);
                case RicochetUserItemTemplate _:
                    return RicochetBattleItemTemplate.CreateEntity(tank);
                default:
                    throw new NotImplementedException();
            }
        }

        protected static Entity CreateEntity(WeaponTemplate template, string configPath, Entity tank)
        {
            return new Entity(new TemplateAccessor(template, configPath.Replace("garage", "battle")),
                tank.GetComponent<TankPartComponent>(),
                new WeaponComponent(),
                // These components should be gotten from configs.
                new WeaponCooldownComponent(1.3f),
                new WeaponRotationComponent(113.667f, 113.333f, 113.333f),
                new WeaponEnergyComponent(1.3f),
                new ShootableComponent(),
                tank.GetComponent<UserGroupComponent>(),
                tank.GetComponent<TankGroupComponent>(),
                tank.GetComponent<BattleGroupComponent>());
        }
    }
}