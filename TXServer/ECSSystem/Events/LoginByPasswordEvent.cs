using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.ECSSystem.Events;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

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
				new FractionGroupComponent(Fractions.Frontier),
				new UserDailyBonusCycleComponent(1),
				new TutorialCompleteIdsComponent(),
				new LeagueGroupComponent(Leagues.Silver),
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
				new ComponentAddCommand(entity, new UserGroupComponent(entity)),
				new EntityShareCommand(Fractions.Competition),
				new EntityShareCommand(Fractions.Frontier),
				new EntityShareCommand(Fractions.Antaeus),
				new SendEventCommand(new UpdateClientFractionScoresEvent(), Fractions.Competition),
				new EntityShareCommand(user)
			};
			
			collectedCommands.AddRange(from global in typeof(BattleRewards).GetFields()
									   select new EntityShareCommand(global.GetValue(null) as Entity));

			collectedCommands.AddRange(from global in typeof(GoldBonuses).GetFields()
									   select new EntityShareCommand(global.GetValue(null) as Entity));

			collectedCommands.AddRange(new Command[] {
				new SendEventCommand(new PaymentSectionLoadedEvent(), entity),
				new ComponentAddCommand(entity, new UserOnlineComponent()),
				new SendEventCommand(new FriendsLoadedEvent(), entity)
			});

			CommandManager.SendCommands(Player.Instance.Socket, collectedCommands.ToArray());
			collectedCommands.Clear();

			collectedCommands.AddRange(from global in typeof(Maps).GetFields()
									   select new EntityShareCommand(global.GetValue(null) as Entity));

			collectedCommands.AddRange(from global in typeof(Containers).GetFields()
									   select new EntityShareCommand(global.GetValue(null) as Entity));

			collectedCommands.AddRange(from global in typeof(Paints).GetFields()
									   select new EntityShareCommand(global.GetValue(null) as Entity));

			collectedCommands.AddRange(from global in typeof(Modules).GetFields()
									   select new EntityShareCommand(global.GetValue(null) as Entity));

			collectedCommands.AddRange(from global in typeof(Shells).GetFields()
									   select new EntityShareCommand(global.GetValue(null) as Entity));

			collectedCommands.AddRange(from global in typeof(WeaponSkins).GetFields()
									   select new EntityShareCommand(global.GetValue(null) as Entity));

			CommandManager.SendCommands(Player.Instance.Socket, collectedCommands.ToArray());
		}

		public string HardwareFingerprint { get; set; }
		public string PasswordEncipher { get; set; }
		public bool RememberMe { get; set; }
	}
}
