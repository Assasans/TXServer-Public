using System;
using System.Collections.Generic;
using TXServer.Core.Commands;
using TXServer.Core.ECSSystem.Components;
using TXServer.Core.ECSSystem.EntityTemplates;
using static TXServer.Core.ECSSystem.Entity;

namespace TXServer.Core.ECSSystem.Events
{
	[SerialVersionUID(1437480091995)]
	public class LoginByPasswordEvent : ECSEvent
	{
		public override void Execute(Entity entity)
		{
			/*
			CommandManager.SendCommands(Player.Instance.Socket,
				new SendEventCommand(new LoginFailedEvent(), entity),
				new SendEventCommand(new InvalidPasswordEvent(), entity));
			*/

			_ = entity ?? throw new ArgumentNullException(nameof(entity));

			Entity user = new Entity(new TemplateAccessor(new UserTemplate(), "quests/daily/battle"),
				new UserXCrystalsComponent(50000),
				new UserCountryComponent("RU"),
				new UserAvatarComponent("457e8f5f-953a-424c-bd97-67d9e116ab7a"), // hardcode!!!
				new UserComponent(),
				new UserMoneyComponent(1000000),
				new FractionGroupComponent(GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_FRONTIER.EntityId),
				new UserDailyBonusCycleComponent(1),
				new TutorialCompleteIdsComponent(),
				new LeagueGroupComponent(GlobalEntities.LEAGUES_LEAGUES_3_SILVER.EntityId),
				new UserDailyBonusZoneComponent(0),
				new UserStatisticsComponent(),
				new PersonalChatOwnerComponent(),
				new GameplayChestScoreComponent(),
				new UserRankComponent(100),
				new BlackListComponent(),
				new UserUidComponent(Player.Instance.Uid),
				new FractionUserScoreComponent(500),
				new UserExperienceComponent(200000),
				new QuestReadyComponent(),
				new UserPublisherComponent(),
				new FavoriteEquipmentStatisticsComponent(),
				new UserGroupComponent(entity.EntityId),
				new UserDailyBonusReceivedRewardsComponent(),
				new ConfirmedUserEmailComponent(Player.Instance.Email),
				new KillsEquipmentStatisticsComponent(),
				new BattleLeaveCounterComponent(),
				new UserReputationComponent(0.0));

			Entity e1 = new Entity(new TemplateAccessor(new xxTemplate(), ""));
			#error

			CommandManager.SendCommands(Player.Instance.Socket,
				new ComponentAddCommand(entity, new UserGroupComponent(entity.EntityId)),
				new EntityShareCommand(GlobalEntities.FRACTIONSCOMPETITION),
				new EntityShareCommand(GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_FRONTIER),
				new EntityShareCommand(GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_ANTAEUS),
				new SendEventCommand(new UpdateClientFractionScoresEvent(), GlobalEntities.FRACTIONSCOMPETITION),
				new EntityShareCommand(user),
				new EntityShareCommand(e1));
		}

		public string HardwareFingerprint { get; set; }
		public string PasswordEncipher { get; set; }
		public bool RememberMe { get; set; }
	}
}
