using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.Library;

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
		public IEnumerable<Command> LastServerPacket { get; set; }
		public List<Command> LastClientPacket { get; set; }
#endif

		public Player(Server server, Socket socket)
        {
            Server = server;
            Connection = new PlayerConnection(this, socket);
			Connection.StartPlayerThreads();
		}

        public void Dispose()
        {
			if (!Connection.TryDeactivate()) return;

			Connection.Dispose();

			foreach (Entity entity in EntityList)
            {
				lock (entity.PlayerReferences)
					entity.PlayerReferences.Remove(this);
            }
			EntityList.Clear();

            //todo save data?
        }

        public bool IsActive => Connection.IsActive;
		public bool TryDeactivate() => Connection.TryDeactivate();

        /// <summary>
        /// Find Entity by id.
        /// </summary>
        public Entity FindEntityById(Int64 id)
        {
            try
            {
                EntityList.TryGetValue(Entity.EqualValue(id), out Entity found);
                return found;
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Entity with id " + id + "not found.");
            }
        }

        public bool LogIn(Entity clientSession)
        {
	        if (GetUniqueId() == null)
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
				new LeagueGroupComponent(Leagues.GlobalItems.Silver),
				new UserStatisticsComponent(),
				new PersonalChatOwnerComponent(),
				new GameplayChestScoreComponent(),
				new UserRankComponent(101),
				new BlackListComponent(),
				new UserUidComponent(Data.UniqueId),
				//new FractionUserScoreComponent(500),
				new UserExperienceComponent(2000000),
				new QuestReadyComponent(),
				new UserPublisherComponent(),
				new FavoriteEquipmentStatisticsComponent(),
				//new UserDailyBonusReceivedRewardsComponent(),
				new ConfirmedUserEmailComponent(Data.Email, Data.Subscribed),
				new UserSubscribeComponent(),
				new KillsEquipmentStatisticsComponent(),
				new BattleLeaveCounterComponent(),
				new UserReputationComponent(0.0));

			if (Data.Admin) user.Components.Add(new UserAdminComponent());
			if (Data.Beta) user.Components.Add(new ClosedBetaQuestAchievementComponent());

			User = user;

			user.Components.Add(new UserGroupComponent(user));

			List<Command> collectedCommands = new List<Command>
			{
				new EntityShareCommand(user),
				new ComponentAddCommand(ClientSession, new UserGroupComponent(user)),
			};
			
			collectedCommands.AddRange(from collectedEntity in ResourceManager.GetEntities(this, user)
									   select new EntityShareCommand(collectedEntity));

			CurrentPreset.WeaponItem.Components.Add(new MountedItemComponent());
			CurrentPreset.HullItem.Components.Add(new MountedItemComponent());

			CurrentPreset.WeaponPaint.Components.Add(new MountedItemComponent());
			CurrentPreset.TankPaint.Components.Add(new MountedItemComponent());

			foreach (Entity item in CurrentPreset.WeaponSkins.Values)
			{
				item.Components.Add(new MountedItemComponent());
			}
			foreach (Entity item in CurrentPreset.HullSkins.Values)
			{
				item.Components.Add(new MountedItemComponent());
			}

			foreach (Entity item in CurrentPreset.WeaponShells.Values)
			{
				item.Components.Add(new MountedItemComponent());
			}
			CurrentPreset.Graffiti.Components.Add(new MountedItemComponent());

			Entity avatar = (UserItems["Avatars"] as Avatars.Items).Tankist;
			avatar.Components.Add(new MountedItemComponent());
			ReferencedEntities.TryAdd("CurrentAvatar", avatar);

			collectedCommands.AddRange(new Command[] {
				//new SendEventCommand(new UpdateClientFractionScoresEvent(), Fractions.GlobalItems.Competition),
				new SendEventCommand(new PaymentSectionLoadedEvent(), user),
				new ComponentAddCommand(user, new UserOnlineComponent()),
				new SendEventCommand(new FriendsLoadedEvent(), ClientSession)
			});

			CommandManager.SendCommands(this, collectedCommands);
			return true;
        }

        public static Int64 GenerateId()
        {
	        Random random = PlayerConnection.Random ?? Server.Instance.Random;
	        return ((long) random.Next() << 32) + random.Next();
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

        public string GetUniqueId()
        {
            return Data?.UniqueId;
        }
    }
}
