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
			Entity entity = new Entity(new TemplateAccessor(new TankTemplate(), hullUserItem.TemplateAccessor.ConfigPath.Replace("garage", "battle")),
				new TankComponent(),
				new TankPartComponent(),
				battleUser.GetComponent<UserGroupComponent>(),
				battleUser.GetComponent<BattleGroupComponent>(),
				new HealthComponent(2500, 2500),
				new HealthConfigComponent(2500),
				new DampingComponent(1500),
				new SpeedComponent(9.967f, 98f, 13.205f),
				new SpeedConfigComponent(112.854f, 19.96f, 10.719f, 226.333f),
				new WeightComponent(2986.667f));

			if (battleUser.GetComponent<TeamGroupComponent>() != null)
            {
				entity.Components.Add(battleUser.GetComponent<TeamGroupComponent>());
			}
			entity.Components.Add(new TankGroupComponent(entity));

			return entity;
		}
	}
}