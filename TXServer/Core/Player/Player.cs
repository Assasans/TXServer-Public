﻿using System;
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
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;
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
				new BattleLeaveCounterComponent(0, 0),
				//new PremiumAccountBoostComponent(endDate: new TXDate(new TimeSpan(12, 0, 0))),
			    new UserReputationComponent(0.0));

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
				new SendEventCommand(new FriendsLoadedEvent(), ClientSession)
			});

			CommandManager.SendCommands(this, collectedCommands);
			return true;
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

        public string GetUniqueId()
        {
            return Data?.UniqueId;
        }

        public override string ToString()
        {
            return (_EndPoint ??= Connection.Socket.RemoteEndPoint).ToString();
        }
		private static EndPoint _EndPoint;
    }
}
