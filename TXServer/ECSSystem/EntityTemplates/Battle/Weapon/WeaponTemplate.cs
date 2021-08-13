using System;
using System.Collections.Generic;
using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Energy;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Battle.Weapon;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1430285417001)]
    public class WeaponTemplate : IEntityTemplate
    {
        private static readonly Dictionary<Type, Func<Entity, BattleTankPlayer, Entity>> UserToBattleTemplate = new()
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
            { typeof(VulcanUserItemTemplate), VulcanBattleItemTemplate.CreateEntity },
        };

        public static Entity CreateEntity(Entity userItem, Entity tank, BattleTankPlayer battlePlayer)
        {
            if (UserToBattleTemplate.TryGetValue(userItem.TemplateAccessor.Template.GetType(),
                out Func<Entity, BattleTankPlayer, Entity> method))
                return method(tank, battlePlayer);

            throw new NotImplementedException();
        }

        protected static Entity CreateEntity(WeaponTemplate template, string configPath, Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity weapon = new(new TemplateAccessor(template, configPath.Replace("garage", "battle")),
                tank.GetComponent<TankPartComponent>(),
                new WeaponComponent(),
                tank.GetComponent<UserGroupComponent>(),
                tank.GetComponent<TankGroupComponent>(),
                tank.GetComponent<BattleGroupComponent>(),
                battlePlayer.Player.CurrentPreset.Weapon.GetComponent<MarketItemGroupComponent>());

            if (template.GetType() != typeof(HammerBattleItemTemplate))
                weapon.AddComponent(new WeaponEnergyComponent(1));

            if (battlePlayer.Team != null)
                weapon.AddComponent(battlePlayer.Team.GetComponent<TeamGroupComponent>());

            if (Config.GetComponent<WeaponCooldownComponent>(configPath, false) is Component component)
            {
                if (battlePlayer.TurretUnloadEnergyPerShot != null)
                    component = new WeaponCooldownComponent((float)battlePlayer.TurretUnloadEnergyPerShot);
                else if (template.GetType() == typeof(ShaftBattleItemTemplate))
                    ((WeaponCooldownComponent) component).CooldownIntervalSec = 2;

                weapon.Components.Add(component);
            }

            return weapon;
        }
    }
}
