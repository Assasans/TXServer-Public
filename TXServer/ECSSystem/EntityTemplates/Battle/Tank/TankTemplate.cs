using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(2012489519776979402)]
    public class TankTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity hullUserItem, Entity battleUser)
        {
            string configPath = hullUserItem.TemplateAccessor.ConfigPath;
            var health = Config.LoadComponent<ServerComponents.HealthComponent>(configPath);

            Entity entity = new(new TemplateAccessor(new TankTemplate(), configPath.Replace("garage", "battle")),
                new TankComponent(),
                new TankPartComponent(),
                battleUser.GetComponent<UserGroupComponent>(),
                battleUser.GetComponent<BattleGroupComponent>(),
                Config.LoadComponent<HealthComponent>(configPath),
                new HealthConfigComponent(health.FinalValue),
                Config.LoadComponent<DampingComponent>(configPath),
                Config.LoadComponent<SpeedComponent>(configPath),
                Config.LoadComponent<SpeedConfigComponent>(configPath),
                Config.LoadComponent<WeightComponent>(configPath),
                new TemperatureComponent(0),
                new TankNewStateComponent());

            if (battleUser.GetComponent<TeamGroupComponent>() != null)
                entity.Components.Add(battleUser.GetComponent<TeamGroupComponent>());
            entity.Components.Add(new TankGroupComponent(entity));

            return entity;
        }
    }
}
