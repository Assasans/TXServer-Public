using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Time;
using System;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    public abstract class BattleTemplate : IEntityTemplate
    {
		protected static Entity CreateEntity(Entity battleLobby, BattleTemplate template, string modeName, int scoreLimit, int timeLimit, int warmingUpTimeLimit)
		{
			UserLimitComponent userLimitComponent = battleLobby.GetComponent<UserLimitComponent>();

			Entity entity = new Entity(new TemplateAccessor(template, "battle/modes/" + modeName),
				new ScoreLimitComponent(scoreLimit),
				new TimeLimitComponent(timeLimit, warmingUpTimeLimit),
				new BattleStartTimeComponent(new DateTimeOffset(DateTime.UtcNow)),
				userLimitComponent,
				battleLobby.GetComponent<MapGroupComponent>(),
				battleLobby.GetComponent<GravityComponent>(),
				battleLobby.GetComponent<BattleModeComponent>(),
				new BattleConfiguratedComponent(),
				new VisibleItemComponent(),
				new UserCountComponent(userLimitComponent.UserLimit),
				new BattleComponent(),
				new BattleTankCollisionsComponent());
			entity.Components.Add(new BattleGroupComponent(entity));

			return entity;
		}
	}
}
