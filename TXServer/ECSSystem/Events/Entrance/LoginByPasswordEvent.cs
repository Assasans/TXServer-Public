﻿using System;
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
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1437480091995)]
	public class LoginByPasswordEvent : ECSEvent
	{
		public void Execute(Entity clientSession)
		{
			if (Player.Instance.Uid == null)
			{
				CommandManager.SendCommands(Player.Instance.Socket,
					new SendEventCommand(new LoginFailedEvent(), clientSession),
					new SendEventCommand(new InvalidPasswordEvent(), clientSession));
				return;
			}

			_ = clientSession ?? throw new ArgumentNullException(nameof(clientSession));

			Entity user = new Entity(new TemplateAccessor(new UserTemplate(), ""),
				new UserXCrystalsComponent(50000),
				new UserCountryComponent("RU"),
				new UserAvatarComponent("8b74e6a3-849d-4a8d-a20e-be3c142fd5e8"),
				new UserComponent(),
				new UserMoneyComponent(1000000),
				//new FractionGroupComponent(Fractions.GlobalItems.Frontier),
				//new UserDailyBonusCycleComponent(1),
				new TutorialCompleteIdsComponent(),
				new RegistrationDateComponent(),
				new LeagueGroupComponent(Leagues.GlobalItems.Silver),
				new UserStatisticsComponent(),
				new PersonalChatOwnerComponent(),
				new GameplayChestScoreComponent(),
				new UserRankComponent(101),
				new BlackListComponent(),
				new UserUidComponent(Player.Instance.Uid),
				//new FractionUserScoreComponent(500),
				new UserExperienceComponent(2000000),
				new QuestReadyComponent(),
				new UserPublisherComponent(),
				new FavoriteEquipmentStatisticsComponent(),
				//new UserDailyBonusReceivedRewardsComponent(),
				new ConfirmedUserEmailComponent("none"),
				new UserSubscribeComponent(),
				new KillsEquipmentStatisticsComponent(),
				new BattleLeaveCounterComponent(),
				new UserReputationComponent(0.0));

			Player.Instance.User = user;

			user.Components.Add(new UserGroupComponent(user.EntityId));

			List<Command> collectedCommands = new List<Command>()
			{
				new EntityShareCommand(user),
				new ComponentAddCommand(clientSession, new UserGroupComponent(user.EntityId)),
			};

			collectedCommands.AddRange(from collectedEntity in ResourceManager.GetEntities(user)
									   select new EntityShareCommand(collectedEntity));

			Player.Instance.CurrentPreset.WeaponItem.Components.Add(new MountedItemComponent());
			Player.Instance.CurrentPreset.HullItem.Components.Add(new MountedItemComponent());

			Player.Instance.CurrentPreset.WeaponPaint.Components.Add(new MountedItemComponent());
			Player.Instance.CurrentPreset.TankPaint.Components.Add(new MountedItemComponent());

			foreach (Entity item in Player.Instance.CurrentPreset.WeaponSkins.Values)
			{
				item.Components.Add(new MountedItemComponent());
			}
			foreach (Entity item in Player.Instance.CurrentPreset.HullSkins.Values)
			{
				item.Components.Add(new MountedItemComponent());
			}

			foreach (Entity item in Player.Instance.CurrentPreset.WeaponShells.Values)
			{
				item.Components.Add(new MountedItemComponent());
			}
			Player.Instance.CurrentPreset.Graffiti.Components.Add(new MountedItemComponent());

			Entity avatar = (Player.Instance.UserItems["Avatars"] as Avatars.Items).Tankist;
			avatar.Components.Add(new MountedItemComponent());
			Player.Instance.ReferencedEntities.TryAdd("CurrentAvatar", avatar);

			collectedCommands.AddRange(new Command[] {
				//new SendEventCommand(new UpdateClientFractionScoresEvent(), Fractions.GlobalItems.Competition),
				new SendEventCommand(new PaymentSectionLoadedEvent(), user),
				new ComponentAddCommand(user, new UserOnlineComponent()),
				new SendEventCommand(new FriendsLoadedEvent(), Player.Instance.ClientSession)
			});

			CommandManager.SendCommands(Player.Instance.Socket, collectedCommands);
		}

		public string HardwareFingerprint { get; set; }
		public string PasswordEncipher { get; set; }
		public bool RememberMe { get; set; }
	}

	[SerialVersionUID(1471252962981L)]
	public class PaymentStatisticsEvent : ECSEvent
	{
		public PaymentStatisticsAction Action { get; set; }

		public long Item { get; set; }

		public long Method { get; set; }

		public string Screen { get; set; }
	}
}
