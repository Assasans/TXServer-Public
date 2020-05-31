using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1437480091995)]
	public class LoginByPasswordEvent : ECSEvent
	{
		public void Execute(Player player, Entity clientSession)
		{
			//todo move this to the player class
			if (player.GetUniqueId() == null)
			{
				CommandManager.SendCommands(player,
					new SendEventCommand(new LoginFailedEvent(), clientSession),
					new SendEventCommand(new InvalidPasswordEvent(), clientSession));
				return;
			}

			_ = clientSession ?? throw new ArgumentNullException(nameof(clientSession));
			PlayerData data = player.Data;
			Console.WriteLine(data.HashedPassword+" "+data.XCrystals+" "+data.CountryCode+" "+data.Avatar+" "+data.Crystals+" "+data.UniqueId+" "+data.Admin+" "+data.Beta);

			Entity user = new Entity(new TemplateAccessor(new UserTemplate(), ""),
				new UserXCrystalsComponent(data.XCrystals),
				new UserCountryComponent(data.CountryCode),
				new UserAvatarComponent(data.Avatar),
				new UserComponent(),
				new UserMoneyComponent(data.Crystals),
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
				new UserUidComponent(player.GetUniqueId()),
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

			if (data.Admin) user.Components.Add(new UserAdminComponent());
			if (data.Beta) user.Components.Add(new ClosedBetaQuestAchievementComponent());

			player.User = user;

			user.Components.Add(new UserGroupComponent(user.EntityId));

			List<Command> collectedCommands = new List<Command>
			{
				new EntityShareCommand(user),
				new ComponentAddCommand(clientSession, new UserGroupComponent(user.EntityId)),
			};

			//todo every entity (weapons, hulls, avatars, containers, covers, daily bonus etc.) is coming from here
			// and maybe it looks like that some things don't have a reference, but then you might not see it because of the
			// Player.Instance. stuff, so we have to search for Player.Instance first (Ctrl+Shift+F) and replace them before
			// deciding that a field for example isn't used
			collectedCommands.AddRange(from collectedEntity in ResourceManager.GetEntities(player, user)
									   select new EntityShareCommand(collectedEntity));

			player.CurrentPreset.WeaponItem.Components.Add(new MountedItemComponent());
			player.CurrentPreset.HullItem.Components.Add(new MountedItemComponent());

			player.CurrentPreset.WeaponPaint.Components.Add(new MountedItemComponent());
			player.CurrentPreset.TankPaint.Components.Add(new MountedItemComponent());

			foreach (Entity item in player.CurrentPreset.WeaponSkins.Values)
			{
				item.Components.Add(new MountedItemComponent());
			}
			foreach (Entity item in player.CurrentPreset.HullSkins.Values)
			{
				item.Components.Add(new MountedItemComponent());
			}

			foreach (Entity item in player.CurrentPreset.WeaponShells.Values)
			{
				item.Components.Add(new MountedItemComponent());
			}
			player.CurrentPreset.Graffiti.Components.Add(new MountedItemComponent());

			Entity avatar = (player.UserItems["Avatars"] as Avatars.Items).Tankist;
			avatar.Components.Add(new MountedItemComponent());
			player.ReferencedEntities.TryAdd("CurrentAvatar", avatar);

			collectedCommands.AddRange(new Command[] {
				//new SendEventCommand(new UpdateClientFractionScoresEvent(), Fractions.GlobalItems.Competition),
				new SendEventCommand(new PaymentSectionLoadedEvent(), user),
				new ComponentAddCommand(user, new UserOnlineComponent()),
				new SendEventCommand(new FriendsLoadedEvent(), player.ClientSession)
			});

			CommandManager.SendCommands(player, collectedCommands);
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
