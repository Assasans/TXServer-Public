using System;
using System.Collections.Generic;
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
        private static Dictionary<Type, Func<Entity, Entity>> userToBattleTemplate = new Dictionary<Type, Func<Entity, Entity>>
        {
            { typeof(SmokyUserItemTemplate), SmokyBattleItemTemplate.CreateEntity },
            { typeof(FlamethrowerUserItemTemplate), FlamethrowerBattleItemTemplate.CreateEntity },
            { typeof(FreezeUserItemTemplate), FreezeBattleItemTemplate.CreateEntity },
            { typeof(IsisUserItemTemplate), IsisBattleItemTemplate.CreateEntity },
            { typeof(ThunderUserItemTemplate), ThunderBattleItemTemplate.CreateEntity },
            { typeof(RicochetUserItemTemplate), RicochetBattleItemTemplate.CreateEntity },
            { typeof(TwinsUserItemTemplate), TwinsBattleItemTemplate.CreateEntity },
            { typeof(RailgunUserItemTemplate), RailgunBattleItemTemplate.CreateEntity },
            { typeof(ShaftUserItemTemplate), ShaftBattleItemTemplate.CreateEntity },
            { typeof(HammerUserItemTemplate), HammerBattleItemTemplate.CreateEntity },
        };

        public static Entity CreateEntity(Entity userItem, Entity tank)
        {
            if (userToBattleTemplate.TryGetValue(userItem.TemplateAccessor.Template.GetType(), out Func<Entity, Entity> method))
                return method(tank);

            throw new NotImplementedException();
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