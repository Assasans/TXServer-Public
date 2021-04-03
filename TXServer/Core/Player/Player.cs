using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.Core.Logging;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;
using TXServer.Library;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core
{
    /// <summary>
    /// Player connection.
    /// </summary>
    public sealed class Player : IDisposable
    {
        public Server Server { get; }
        public PlayerConnection Connection { get; }
        public PlayerData Data { get; set; }

#if DEBUG
		public IEnumerable<ICommand> LastServerPacket { get; set; }
		public List<ICommand> LastClientPacket { get; set; }
#endif

		public Player(Server server, Socket socket)
        {
            Server = server;
            Connection = new PlayerConnection(this, socket);
			Connection.StartPlayerThreads();

			Logger.Log($"{this} has connected.");
		}

        public void Dispose()
        {
			if (!Connection.IsActive) return;

			Connection.Dispose();

			foreach (Entity entity in EntityList)
            {
				lock (entity.PlayerReferences)
					entity.PlayerReferences.Remove(this);
            }
			EntityList.Clear();

			Logger.Log($"{this} has disconnected.");

			//todo save data?
		}

        public bool IsActive => Connection.IsActive;
		public bool TryDeactivate() => Connection.TryDeactivate();

        /// <summary>
        /// Find Entity by id.
        /// </summary>
        public Entity FindEntityById(Int64 id)
        {
            EntityList.TryGetValue(Entity.EqualValue(id), out Entity found);
			return found;
        }

        public bool LogIn(Entity clientSession)
        {
	        if (UniqueId == null)
			{
				CommandManager.SendCommands(this,
					new SendEventCommand(new LoginFailedEvent(), ClientSession),
					new SendEventCommand(new InvalidPasswordEvent(), ClientSession));
				return false;
			}

	        if (ClientSession.EntityId != clientSession?.EntityId)
	        {
		        throw new ArgumentException("ClientSession Entity doesn't match Player ClientSession Entity");
	        }

			Logger.Log($"{this}: Logged in as {Data.UniqueId}.");

			Entity user = new Entity(new TemplateAccessor(new UserTemplate(), ""),
				new UserXCrystalsComponent(Data.XCrystals),
				new UserCountryComponent(Data.CountryCode),
				new UserAvatarComponent(Data.Avatar),
				new UserComponent(),
				new UserMoneyComponent(Data.Crystals),
				//new FractionGroupComponent(Fractions.GlobalItems.Frontier),
				//new UserDailyBonusCycleComponent(1),
				new TutorialCompleteIdsComponent(),
				new RegistrationDateComponent(),
				new LeagueGroupComponent(Leagues.GlobalItems.Master),
				new UserStatisticsComponent(),
				new PersonalChatOwnerComponent(),
				new GameplayChestScoreComponent(),
				new UserRankComponent(1),
				new BlackListComponent(),
				new UserUidComponent(Data.UniqueId),
				//new FractionUserScoreComponent(500),
				new UserExperienceComponent(0),
				new QuestReadyComponent(),
				new UserPublisherComponent(),
				new FavoriteEquipmentStatisticsComponent(),
				//new UserDailyBonusReceivedRewardsComponent(),
				new ConfirmedUserEmailComponent(Data.Email, Data.Subscribed),
				new UserSubscribeComponent(),
				new KillsEquipmentStatisticsComponent(),
				new BattleLeaveCounterComponent(0, 0),
				//new PremiumAccountBoostComponent(endDate: new TXDate(new TimeSpan(12, 0, 0))),
			    new UserReputationComponent(0.0));

			// temp solution
			List<string> AdminUids = new() { "NoNick", "Tim203", "M8", "Kaveman"};
			if (!AdminUids.Contains(Data.UniqueId))
				Data.Admin = false;

			if (Data.Admin)
			{
				user.Components.Add(new UserAdminComponent());
			    if (user.GetComponent<PremiumAccountBoostComponent>() == null)
                {
					user.Components.Add(new PremiumAccountBoostComponent(endDate: new TXDate(new TimeSpan(23999976, 0, 0))));
                }
			}
			if (Data.Beta) user.Components.Add(new ClosedBetaQuestAchievementComponent());

			User = user;

			user.Components.Add(new UserGroupComponent(user));

			List<ICommand> collectedCommands = new List<ICommand>
			{
				new EntityShareCommand(user),
				new ComponentAddCommand(ClientSession, new UserGroupComponent(user)),
			};
			
			collectedCommands.AddRange(from collectedEntity in ResourceManager.GetEntities(this, user)
									   select new EntityShareCommand(collectedEntity));

			collectedCommands.AddRange(new[]
			{
				new ComponentAddCommand(CurrentPreset.WeaponItem, new MountedItemComponent()),
				new ComponentAddCommand(CurrentPreset.HullItem, new MountedItemComponent()),

				new ComponentAddCommand(CurrentPreset.WeaponPaint, new MountedItemComponent()),
				new ComponentAddCommand(CurrentPreset.TankPaint, new MountedItemComponent())
			});

			foreach (Entity item in CurrentPreset.WeaponSkins.Values
				                    .Concat(CurrentPreset.HullSkins.Values)
									.Concat(CurrentPreset.WeaponShells.Values))
			{
				collectedCommands.Add(new ComponentAddCommand(item, new MountedItemComponent()));
			}

			collectedCommands.Add(new ComponentAddCommand(CurrentPreset.Graffiti, new MountedItemComponent()));

			Entity avatar = (UserItems["Avatars"] as Avatars.Items).Tankist;
			collectedCommands.Add(new ComponentAddCommand(avatar, new MountedItemComponent()));
			ReferencedEntities.TryAdd("CurrentAvatar", avatar);

			collectedCommands.AddRange(new ICommand[] {
				//new SendEventCommand(new UpdateClientFractionScoresEvent(), Fractions.GlobalItems.Competition),
			    new SendEventCommand(new PaymentSectionLoadedEvent(), user),
				new ComponentAddCommand(user, new UserOnlineComponent()),
				new SendEventCommand(new FriendsLoadedEvent(this), ClientSession)
			});

			CommandManager.SendCommands(this, collectedCommands);
			return true;
        }

		public void CheckRankUp()
        {
			// todo: load this list from configs
			List<long> experienceForRank = new() {0, 200, 800, 1800, 3200, 5000, 7200, 9800, 12800, 16200, 20000, 24200, 28800, 33800, 39200, 
				45000, 51200, 57800, 64800, 72200, 80000, 88200, 96800, 105800, 115200, 125000, 135200, 145800, 156800, 168200, 180000, 192200, 
				204800, 217800, 231200, 245000, 259200, 273800, 288800, 304200, 320000, 336200, 352800, 369800, 387200, 405000, 423200, 441800, 
				460800, 480200, 500000, 520200, 540800, 561800, 583200, 605000, 627200, 649800, 672800, 696200, 720000, 744200, 768800, 793800, 
				819200, 845000, 871200, 897800, 924800, 952200, 980000, 1008200, 1036800, 1065800, 1095200, 1125000, 1155200, 1185800, 1216800, 
				1248200, 1280000, 1312200, 1344800, 1377800, 1411200, 1445000, 1479200, 1513800, 1548800, 1584200, 1620000, 1656200, 1692800, 
				1729800, 1767200, 1805000, 1843200, 1881800, 1920800, 1960200, 2000000
			};

			long totalExperience = User.GetComponent<UserExperienceComponent>().Experience;
			if (IsInMatch())
            {
				int battleExperience = BattlePlayer.MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses;
				if (User.GetComponent<PremiumAccountBoostComponent>() == null)
					totalExperience += battleExperience;
				else
					totalExperience += battleExperience * 2;
			}
				
			int correctRank = experienceForRank.IndexOf(experienceForRank.LastOrDefault(x => x <= totalExperience)) + 1;

			if (User.GetComponent<UserRankComponent>().Rank < correctRank)
            {
				User.ChangeComponent(new UserRankComponent(correctRank));
				// todo: load rank rewards from configs (https://vignette2.wikia.nocookie.net/tanki-x/images/f/fb/Rankit.png/revision/latest?cb=20170629172052)
				Entity rankUpNotification = UserRankRewardNotificationTemplate.CreateEntity(100, 5000, correctRank);
				ShareEntity(rankUpNotification);

				if (IsInMatch())
                {
					BattlePlayer.Battle.MatchPlayers.Select(x => x.Player).SendEvent(new UpdateRankEvent(), User);
					int currentScoreInBattle = BattlePlayer.MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses;
					User.ChangeComponent<UserExperienceComponent>(component => component.Experience += currentScoreInBattle);
					BattlePlayer.MatchPlayer.AlreadyAddedExperience += currentScoreInBattle;
				}
			}
        }

		public bool IsLoggedIn()
        {
			if (User != null) return true;
			return false;
        }

		public bool IsInBattleLobby()
        {
			if (BattlePlayer != null) return true;
			else return false;
		}

		public bool IsInMatch()
        {
			if (IsInBattleLobby() && BattlePlayer.MatchPlayer != null) return true;
			else return false;
        }

		public bool IsOwner()
        {
			if (IsInBattleLobby() && BattlePlayer.Battle.TypeHandler is CustomBattleHandler)
				if ((BattlePlayer.Battle.TypeHandler as CustomBattleHandler).Owner == this)
					return true;
			return false;
        }

		public void SendEvent(ECSEvent @event, params Entity[] entities)
        {
			CommandManager.SendCommandsSafe(this, new SendEventCommand(@event, entities));
        }

		public void ShareEntity(Entity entity)
		{
			AddEntity(entity);
			CommandManager.SendCommandsSafe(this, new EntityShareCommand(entity, isManual: true));
		}

		public void ShareEntities(params Entity[] entities) => ShareEntities((IEnumerable<Entity>)entities);
		public void ShareEntities(IEnumerable<Entity> entities)
        {
			foreach (Entity entity in entities)
				ShareEntity(entity);
        }

		public void UnshareEntity(Entity entity)
		{
			RemoveEntity(entity);
			CommandManager.SendCommandsSafe(this, new EntityUnshareCommand(entity, isManual: true));
		}

		public void UnshareEntities(params Entity[] entities) => UnshareEntities((IEnumerable<Entity>)entities);
		public void UnshareEntities(IEnumerable<Entity> entities)
		{
			foreach (Entity entity in entities)
				UnshareEntity(entity);
		}

		public void AddEntity(Entity entity)
        {
			EntityList.Add(entity);
			entity.PlayerReferences.Add(this);
		}

		public void RemoveEntity(Entity entity)
        {
			EntityList.Remove(entity);
			entity.PlayerReferences.Remove(this);
		}

        public ConcurrentHashSet<Entity> EntityList { get; } = new ConcurrentHashSet<Entity>();

        /// <summary>
        /// Use for short-timed entity references from unconnected places.
		/// For long-timed ones consider using properties.
        /// </summary>
        public ConcurrentDictionary<string, Entity> ReferencedEntities { get; } = new ConcurrentDictionary<string, Entity>();
        
        //todo add those two in PlayerData
        public ConcurrentDictionary<string, ItemList> UserItems { get; } = new ConcurrentDictionary<string, ItemList>();
        public PresetEquipmentComponent CurrentPreset { get; set; }

        public Entity ClientSession { get; set; }

        public Entity User { get; set; }
		public BattlePlayer BattlePlayer { get; set; }

        public string UniqueId => Data?.UniqueId;

        public override string ToString()
        {
			return $"{_EndPoint ??= Connection.Socket.RemoteEndPoint}{(ClientSession != null ? $" ({ClientSession.EntityId}{(UniqueId != null ? $", {UniqueId}" : "")})" : "")}";
		}
		private EndPoint _EndPoint;
    }
}
