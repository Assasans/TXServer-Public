using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.ECSSystem.Events;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using static TXServer.ECSSystem.Base.Entity;

namespace TXServer.ECSSystem.Events
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
				new FractionGroupComponent(GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_FRONTIER),
				new UserDailyBonusCycleComponent(1),
				new TutorialCompleteIdsComponent(),
				new LeagueGroupComponent(GlobalEntities.LEAGUES_LEAGUES_3_SILVER),
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
				new UserGroupComponent(entity),
				new UserDailyBonusReceivedRewardsComponent(),
				new ConfirmedUserEmailComponent(Player.Instance.Email),
				new KillsEquipmentStatisticsComponent(),
				new BattleLeaveCounterComponent(),
				new UserReputationComponent(0.0));

			List<Command> collectedCommands = new List<Command>()
			{
				new ComponentAddCommand(entity, new UserGroupComponent(entity /* replace with random group id 1 */)),
				new EntityShareCommand(GlobalEntities.FRACTIONSCOMPETITION),
				new EntityShareCommand(GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_FRONTIER),
				new EntityShareCommand(GlobalEntities.FRACTIONSCOMPETITION_FRACTIONS_ANTAEUS),
				new SendEventCommand(new UpdateClientFractionScoresEvent(), GlobalEntities.FRACTIONSCOMPETITION),
				new EntityShareCommand(user)
			};

			collectedCommands.AddRange(from global in typeof(GlobalEntities).GetFields()
									   where global.Name.Contains("BATTLE_MAP")
									   select new EntityShareCommand(global.GetValue(null) as Entity));

			collectedCommands.AddRange(from global in typeof(GlobalEntities).GetFields()
									   where global.Name.Contains("BATTLE_REWARDS")
									   select new EntityShareCommand(global.GetValue(null) as Entity));

			collectedCommands.Add(new SendEventCommand(new PaymentSectionLoadedEvent(), entity /* replace with random group id 1 */));

			collectedCommands.AddRange(from global in typeof(GlobalEntities).GetFields()
									   where global.Name.Contains("PAYMENT_GOODS_GOLDBONUS")
									   select new EntityShareCommand(global.GetValue(null) as Entity));

			CommandManager.SendCommands(Player.Instance.Socket, collectedCommands.ToArray());
		}

		public string HardwareFingerprint { get; set; }
		public string PasswordEncipher { get; set; }
		public bool RememberMe { get; set; }
	}
}
