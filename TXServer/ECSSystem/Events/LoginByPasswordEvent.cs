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
				new UserAvatarComponent("8b74e6a3-849d-4a8d-a20e-be3c142fd5e8"),
				new UserComponent(),
				new UserMoneyComponent(1000000),
				new FractionGroupComponent(Fractions.GlobalItems.Frontier),
				new UserDailyBonusCycleComponent(1),
				new TutorialCompleteIdsComponent(),
				new RegistrationDateComponent(),
				new LeagueGroupComponent(Leagues.GlobalItems.Silver),
				new UserDailyBonusNextReceivingDateComponent(),
				new UserDailyBonusInitializedComponent(),
				new UserDailyBonusZoneComponent(0),
				new UserStatisticsComponent(),
				new PersonalChatOwnerComponent(),
				new GameplayChestScoreComponent(),
				new UserRankComponent(100),
				new BlackListComponent(),
				new UserUidComponent(Player.Instance.Uid),
				new FractionUserScoreComponent(500),
				new UserExperienceComponent(1000000),
				new QuestReadyComponent(),
				new UserPublisherComponent(),
				new FavoriteEquipmentStatisticsComponent(),
				new UserDailyBonusReceivedRewardsComponent(),
				new ConfirmedUserEmailComponent(Player.Instance.Email),
				new UserSubscribeComponent(),
				new KillsEquipmentStatisticsComponent(),
				new BattleLeaveCounterComponent(),
				new UserReputationComponent(0.0));

			user.Components.Add(new UserGroupComponent(user.EntityId));

			List<Command> collectedCommands = new List<Command>()
			{
				new EntityShareCommand(user),
				new ComponentAddCommand(entity, new UserGroupComponent(user.EntityId)),
			};

			collectedCommands.AddRange(from collectedEntity in ResourceManager.GetEntities(user)
									   select new EntityShareCommand(collectedEntity));

			collectedCommands.AddRange(new Command[] {
				new SendEventCommand(new UpdateClientFractionScoresEvent(), Fractions.GlobalItems.Competition),
				new SendEventCommand(new PaymentSectionLoadedEvent(), user),
				new ComponentAddCommand(user, new UserOnlineComponent()),
				new SendEventCommand(new FriendsLoadedEvent(), user)
			});

			CommandManager.SendCommands(Player.Instance.Socket, collectedCommands.ToArray());
		}

		public string HardwareFingerprint { get; set; }
		public string PasswordEncipher { get; set; }
		public bool RememberMe { get; set; }
	}

	[SerialVersionUID(1497606008074L)]
	public class UserQuestReadyEvent : ECSEvent
	{
	}

	[SerialVersionUID(1464349204724L)]
	public class ClientInfoSendEvent : ECSEvent
	{
		public string Settings { get; set; }
	}

	[SerialVersionUID(1507022246767L)]
	public class UserOnlineEvent : ECSEvent
	{
	}

	[SerialVersionUID(5356229304896471086L)]
	public class PingEvent : ECSEvent
	{
		public long ServerTime { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();

		public byte CommandId { get; set; } = 0xf0;
	}

	[SerialVersionUID(1115422024552825915L)]
	public class PongEvent : ECSEvent
	{
		public float PongCommandClientRealTime { get; set; }

		public byte CommandId { get; set; }
	}
}
