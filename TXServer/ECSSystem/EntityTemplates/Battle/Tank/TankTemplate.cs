using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Tank
{
    [SerialVersionUID(2012489519776979402)]
    public class TankTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity hullUserItem, Entity battleUser, BattleTankPlayer battlePlayer)
        {
            string configPath = hullUserItem.TemplateAccessor.ConfigPath;

            Entity entity = new(new TemplateAccessor(new TankTemplate(), configPath.Replace("garage", "battle")),
                new TankComponent(),
                new TankPartComponent(),
                battleUser.GetComponent<UserGroupComponent>(),
                battleUser.GetComponent<BattleGroupComponent>(),
                Config.GetComponent<HealthConfigComponent>(configPath),
                Config.GetComponent<DampingComponent>(configPath),
                Config.GetComponent<SpeedComponent>(configPath),
                Config.GetComponent<SpeedConfigComponent>(configPath),
                Config.GetComponent<WeightComponent>(configPath),
                new TemperatureComponent(0),
                new TankNewStateComponent(),
                battlePlayer.Player.CurrentPreset.Hull.GetComponent<MarketItemGroupComponent>());

            if (battleUser.HasComponent<TeamGroupComponent>())
                entity.Components.Add(battleUser.GetComponent<TeamGroupComponent>());
            entity.Components.Add(new TankGroupComponent(entity));

            return entity;
        }
    }
}
