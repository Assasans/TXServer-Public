using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.Core.Logging;
using TXServer.Core.Squads;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.User;
using TXServer.ECSSystem.Events;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.Library;
using static TXServer.Core.Battles.Battle;
using TXServer.Core.Database;
using TXDatabase.NetworkEvents.Communications;
using TXServer.ECSSystem.EntityTemplates.Notification;
using TXServer.ECSSystem.EntityTemplates.Notification.FractionsCompetition;
using TXServer.ECSSystem.EntityTemplates.User;
using TXServer.ECSSystem.Events.Fraction;
using TXServer.ECSSystem.Types;

namespace TXServer.Core
{
    /// <summary>
    /// Player connection.
    /// </summary>
    public sealed class Player : IDisposable
    {
        public bool IsLoggedIn => User != null;

        public bool IsInSquad => SquadPlayer != null;
        public bool IsSquadLeader => IsInSquad && SquadPlayer.IsLeader;

        public bool IsInBattle => BattlePlayer != null;
        public bool IsBattleOwner => (BattlePlayer?.Battle.TypeHandler as CustomBattleHandler)?.Owner == this;
        public bool IsInMatch => BattlePlayer?.MatchPlayer != null;

        public bool IsBanned => Data.Punishments.Any(p => p.Type is PunishmentType.Ban && p.IsActive);
        public bool IsMuted => Data.Punishments.Any(p => p.Type is PunishmentType.Mute && p.IsActive);

        public bool HasEntityWithId(long id, out Entity searchedEntity)
        {
            searchedEntity = GetEntityById(id);
            return searchedEntity is not null;
        }

        // todo add that in PlayerData
        public ConcurrentDictionary<Type, ItemList> UserItems { get; } = new();


        public PresetEquipmentComponent CurrentPreset =>
            Data.Presets.Single(p => p.HasComponent<MountedItemComponent>())
                .GetComponent<PresetEquipmentComponent>();
        public PresetEquipmentComponent RestorablePreset { get; set; }

        public Entity ClientSession { get; set; }
        public Entity User { get; set; }
        public BattleTankPlayer BattlePlayer { get; set; }
        public Spectator Spectator { get; set; }
        public SquadPlayer SquadPlayer { get; set; }
        public RSAEncryptionComponent EncryptionComponent { get; } = new();

        public ConcurrentHashSet<Entity> EntityList { get; } = new();

        private readonly ConcurrentDictionary<Player, int> _SharedPlayers = new();

        public string UniqueId => Data?.Username;
        public ServerData ServerData => Server.Instance.ServerData;

        public Server Server { get; }
        public PlayerConnection Connection { get; }
        public PlayerData Data { get; set; }
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
                lock (entity.PlayerReferences)
                    entity.PlayerReferences.Remove(this);

            EntityList.Clear();

            Logger.Log($"{this} has disconnected.");

            Server.StoredPlayerData.RemoveAll(pd => pd.UniqueId == Data?.UniqueId);
            if (Data is {RememberMe: true})
                Server.StoredPlayerData.Add(Data);
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

        public void LogInWithDatabase(Entity clientSession)
        {
            if (!Server.DatabaseNetwork.IsReady)
            {
                LogIn(clientSession);
                return;
            }
            Logger.Log($"{this}: Loading data from database {Data.Username} ({Data.UniqueId}).");

            // Time to block chain all the stuff ;-;
            PlayerDataProxy dataProxy = (PlayerDataProxy)Data;
            PacketSorter.GetUserSettings(Data.UniqueId, settingsData =>
            {
                dataProxy.SetSettings(
                    Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(settingsData.countryCode),
                    new DateTime(settingsData.premiumExpiration),
                    settingsData.subscribed
                );
                // Request the other stuff here.... I think stats is next?
                // After your done, in the last block chain call LogIn()
                Server.DatabaseNetwork.Socket.emit(new UserLoggedInEvent() { uid = Data.UniqueId });
                LogIn(clientSession);
            });
        }

        public bool LogIn(Entity clientSession)
        {
            if (UniqueId == null)
            {
                SendEvent(new LoginFailedEvent(), ClientSession);
                SendEvent(new InvalidPasswordEvent(), ClientSession);
                return false;
            }

            if (ClientSession.EntityId != clientSession?.EntityId)
                throw new ArgumentException("ClientSession Entity doesn't match Player ClientSession Entity");

            Logger.Log($"{this}: Logged in as {Data.Username}.");

            // todo: in db
            List<string> admins = new() { "NoNick", "Tim203", "M8", "Kaveman", "Assasans" };
            if (!admins.Contains(Data.Username))
            {
                Data.Admin = false;
                Data.Mod = false;
            }

            Entity user = UserTemplate.CreateEntity(this);
            User = user;

            ShareEntities(user);
            clientSession.AddComponent(user.GetComponent<UserGroupComponent>());

            ShareEntities(ResourceManager.GetEntities(this, user));
            SetEquipment();

            foreach (Entity item in new[]
            {
                GetUserItemByMarket(GetEntityById(Data.Avatar)),
                CurrentPreset.WeaponItem,
                CurrentPreset.HullItem,
                CurrentPreset.WeaponPaint,
                CurrentPreset.TankPaint,
                CurrentPreset.Graffiti,
            }.Concat(CurrentPreset.HullSkins.Values)
             .Concat(CurrentPreset.WeaponSkins.Values)
             .Concat(CurrentPreset.WeaponShells.Values))
            {
                item.AddComponent(new MountedItemComponent());
            }

            SendEvent(new PaymentSectionLoadedEvent(), user);
            SendEvent(new FriendsLoadedEvent(this), ClientSession);

            return true;
        }

        public void CheckRankUp() => Leveling.CheckRankUp(this);
        public void CheckTankRankUp(Entity item) => Leveling.CheckTankRankUp(item, this);

        public void CheckRecruitReward()
        {
            if (Data.LastRecruitReward is not null &&
                (DateTimeOffset.UtcNow - Data.LastRecruitReward).Value.TotalHours < 24 ||
                Data.RecruitRewardDay > 14)
                return;

            Data.LastRecruitReward = DateTimeOffset.UtcNow;
            Data.RecruitRewardDay++;

            Entity notification = LoginRewardNotificationTemplate.CreateEntity(this);
            ShareEntities(notification);
            SendEvent(new ShowNotificationGroupEvent(1), notification);
        }

        public void CheckNotifications()
        {
            if (ServerData.FractionsCompetitionFinished && !Data.ReceivedFractionsCompetitionReward &&
                Data.Fraction is not null)
                ShareEntities(FractionsCompetitionRewardNotificationTemplate.CreateEntity(this));

            // release gift notification
            if (ServerData.SpreadReleaseGift && !Data.ReceivedReleaseReward &&
                Data.RegistrationDate <= ServerData.ReleaseGiftMaxRegistrationDate)
                ShareEntities(ReleaseGiftsPersistentNotificationTemplate.CreateEntity(this));

            Fractions.CheckForNotifications(this);
            Leagues.CheckForNotifications(this);
        }

        public void UpdateFractionScores() =>
            SendEvent(new UpdateClientFractionScoresEvent(), Fractions.GlobalItems.Competition);

        public Entity GetMarketItemByUser(Entity userItem) => ResourceManager.GetMarketItem(this, userItem);
        public Entity GetUserItemByMarket(Entity marketItem) => ResourceManager.GetUserItem(this, marketItem);
        public int GetUserItemLevel(Entity userItem) => ResourceManager.GetUserItemLevel(this, userItem);

        public void ScreenMessage(string message, bool clearOtherMessages = true)
        {
            if (clearOtherMessages)
                foreach (Entity entity in EntityList.Where(e =>
                    e.TemplateAccessor.Template.GetType() == typeof(SimpleTextNotificationTemplate)))
                    UnshareEntities(entity);

            Entity notification = SimpleTextNotificationTemplate.CreateEntity(message);
            ShareEntities(notification);
        }

        public void SaveNewMarketItem(Entity marketItem, int amount = 1) =>
            ResourceManager.SaveNewMarketItem(this, marketItem, amount);

        public void SetEquipment()
        {
            User.AddComponent(new UserAvatarComponent(Data.Avatar, this));
        }

        /// <summary>
        /// Shares users of players.
        /// </summary>
        /// <param name="players">Players with users to be shared.</param>
        public void SharePlayers(params Player[] players) => SharePlayers((IEnumerable<Player>)players);
        /// <summary>
        /// Shares users of players.
        /// </summary>
        /// <param name="players">Players with users to be shared.</param>
        public void SharePlayers(IEnumerable<Player> players)
        {
            foreach (Player player in players)
            {
                if (player.User == User)
                    throw new ArgumentException("Self player cannot be shared.");

                if (_SharedPlayers.AddOrUpdate(player, 1, (key, value) => ++value) == 1 && IsActive)
                    ShareEntities(player.User);
            }
        }

        /// <summary>
        /// Unshares users of players.
        /// </summary>
        /// <param name="players">Players with users to be unshared.</param>
        public void UnsharePlayers(params Player[] players) => UnsharePlayers((IEnumerable<Player>)players);
        /// <summary>
        /// Unshares users of players.
        /// </summary>
        /// <param name="players">Players with users to be unshared.</param>
        public void UnsharePlayers(IEnumerable<Player> players)
        {
            foreach (Player player in players)
            {
                if (player.User == User)
                    throw new ArgumentException("Self player cannot be unshared.");

                if (_SharedPlayers.TryRemove(new KeyValuePair<Player, int>(player, 1)))
                {
                    if (IsActive)
                        UnshareEntities(player.User);
                }
                else
                {
                    _SharedPlayers.AddOrUpdate(player, player => throw new InvalidOperationException("Player is not shared."), (key, value) => --value);
                }
            }
        }

        public void SendEvent(ECSEvent @event, params Entity[] entities)
        {
            Connection.QueueCommands(new SendEventCommand(@event, entities));
        }

        public void ShareEntities(params Entity[] entities) => ShareEntities((IEnumerable<Entity>)entities);
        public void ShareEntities(IEnumerable<Entity> entities)
        {
            foreach (Entity entity in entities)
            {
                EntityList.Add(entity);
                entity.PlayerReferences.Add(this);
                Connection.QueueCommands(new EntityShareCommand(entity, this));
            }
        }

        public void UnshareEntities(params Entity[] entities) => UnshareEntities((IEnumerable<Entity>)entities);
        public void UnshareEntities(IEnumerable<Entity> entities)
        {
            foreach (Entity entity in entities)
            {
                EntityList.Remove(entity);
                entity.PlayerReferences.Remove(this);
                Connection.QueueCommands(new EntityUnshareCommand(entity));
            }
        }

        public Entity GetEntityById(long id) => EntityList.SingleOrDefault(e => e.EntityId == id);

        public void Tick()
        {
            foreach ((Entity notification, DateTimeOffset timeLimit) in Notifications.ToList())
            {
                if (DateTimeOffset.UtcNow <= timeLimit) continue;
                Notifications.Remove(notification);
                UnshareEntities(notification);
            }
        }

        public override string ToString() => $"{_EndPoint ??= Connection.Socket.RemoteEndPoint}{(ClientSession != null ? $" ({ClientSession.EntityId}{(UniqueId != null ? $", {UniqueId}" : null)})" : null)}";
        private EndPoint _EndPoint;

        public readonly Dictionary<Entity, DateTimeOffset> Notifications = new();
    }
}
