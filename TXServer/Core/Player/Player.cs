using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.Core.Logging;
using TXServer.Core.Squads;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.User;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.Library;
using static TXServer.Core.Battles.Battle;
using TXServer.Core.Database;
using TXServer.Core.Database.NetworkEvents.Communications;

namespace TXServer.Core
{
    /// <summary>
    /// Player connection.
    /// </summary>
    public sealed class Player : IDisposable
    {
        public bool IsLoggedIn => User != null;
        public bool IsPremium => Data.PremiumExpirationDate > DateTime.UtcNow;

        public bool IsInSquad => SquadPlayer != null;
        public bool IsSquadLeader => IsInSquad && SquadPlayer.IsLeader;

        public bool IsInBattle => BattlePlayer != null;
        public bool IsBattleOwner => (BattlePlayer?.Battle.TypeHandler as CustomBattleHandler)?.Owner == this;
        public bool IsInMatch => BattlePlayer?.MatchPlayer != null;

        //todo add those three in PlayerData
        public Entity CurrentAvatar { get; set; }
        public ConcurrentDictionary<Type, ItemList> UserItems { get; } = new();
        public PresetEquipmentComponent CurrentPreset { get; set; }

        public Entity ClientSession { get; set; }
        public Entity User { get; set; }
        public BattleTankPlayer BattlePlayer { get; set; }
        public Spectator Spectator { get; set; }
        public SquadPlayer SquadPlayer { get; set; }
        public RSAEncryptionComponent EncryptionComponent { get; } = new RSAEncryptionComponent();

        public ConcurrentHashSet<Entity> EntityList { get; } = new ConcurrentHashSet<Entity>();
        public IReadOnlyCollection<Player> SharedPlayers => (IReadOnlyCollection<Player>)_SharedPlayers.Keys;
        private readonly ConcurrentDictionary<Player, int> _SharedPlayers = new();

        public string UniqueId => Data?.Username;

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

        public void LogInWithDatabase(Entity clientSession)
        {
            if (!Server.DatabaseNetwork.isReady)
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
                    Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(settingsData.avatar),
                    DateTime.ParseExact(
                        Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(settingsData.premiumExpiration),
                        "HH:mm:ss MM/dd/yyyy", CultureInfo.InvariantCulture),
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
            // I did something similiar... lemme find it
            Entity user = new(new TemplateAccessor(new UserTemplate(), ""),
                new UserCountryComponent(Data.CountryCode),
                new UserAvatarComponent(Data.Avatar),
                new UserComponent(),
                //new FractionGroupComponent(Fractions.GlobalItems.Frontier),
                //new UserDailyBonusCycleComponent(1),
                new TutorialCompleteIdsComponent(),
                new RegistrationDateComponent(),
                new LeagueGroupComponent(Leagues.GlobalItems.Master),
                new UserStatisticsComponent(),
                new PersonalChatOwnerComponent(),
                new GameplayChestScoreComponent(),
                new BlackListComponent(),
                new UserUidComponent(Data.Username),
                //new FractionUserScoreComponent(500),

                new UserMoneyComponent(Data.Crystals),
                new UserXCrystalsComponent(Data.XCrystals),
                new UserRankComponent(1),
                new UserExperienceComponent(Data.Experience),
                new UserReputationComponent(Data.Reputation),

                /*
				new UserDailyBonusCycleComponent(1),
				new UserDailyBonusReceivedRewardsComponent(),
				new UserDailyBonusZoneComponent(1),
				new UserDailyBonusNextReceivingDateComponent(),
				new UserDailyBonusInitializedComponent(),
				*/

                new QuestReadyComponent(),
                new UserPublisherComponent(),
                new FavoriteEquipmentStatisticsComponent(),
                new ConfirmedUserEmailComponent(Data.Email, Data.Subscribed),
                new UserSubscribeComponent(),
                new KillsEquipmentStatisticsComponent(),
                new BattleLeaveCounterComponent(0, 0),
                new UserOnlineComponent());
            user.Components.Add(new UserGroupComponent(user));

            User = user;

            // temp solution
            List<string> admins = new() { "NoNick", "Tim203", "M8", "Kaveman", "Assasans" };
            if (!admins.Contains(Data.Username))
                Data.Admin = false;

            // tip: don't change this order
            if (Data.PremiumExpirationDate > DateTime.UtcNow)
                user.AddComponent(new PremiumAccountBoostComponent { EndDate = Data.PremiumExpirationDate });
            if (Data.Admin)
            {
                user.Components.Add(new UserAdminComponent());
                Data.RenewPremium(new TimeSpan(23999976, 0, 0));
            }
            if (Data.Beta) user.Components.Add(new ClosedBetaQuestAchievementComponent());


            ShareEntities(user);
            clientSession.AddComponent(new UserGroupComponent(user));

            ShareEntities(ResourceManager.GetEntities(this, user));

            CurrentAvatar = ((Avatars.Items)UserItems[typeof(Avatars)]).Tankist;
            foreach (Entity item in new[]
            {
                CurrentAvatar,
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

            //new SendEventCommand(new UpdateClientFractionScoresEvent(), Fractions.GlobalItems.Competition),
            SendEvent(new PaymentSectionLoadedEvent(), user);
            SendEvent(new FriendsLoadedEvent(this), ClientSession);

            return true;
        }

        public void CheckRankUp()
        {
            // todo: load this list from configs
            List<long> experienceForRank = new()
            {
                0,
                200,
                800,
                1800,
                3200,
                5000,
                7200,
                9800,
                12800,
                16200,
                20000,
                24200,
                28800,
                33800,
                39200,
                45000,
                51200,
                57800,
                64800,
                72200,
                80000,
                88200,
                96800,
                105800,
                115200,
                125000,
                135200,
                145800,
                156800,
                168200,
                180000,
                192200,
                204800,
                217800,
                231200,
                245000,
                259200,
                273800,
                288800,
                304200,
                320000,
                336200,
                352800,
                369800,
                387200,
                405000,
                423200,
                441800,
                460800,
                480200,
                500000,
                520200,
                540800,
                561800,
                583200,
                605000,
                627200,
                649800,
                672800,
                696200,
                720000,
                744200,
                768800,
                793800,
                819200,
                845000,
                871200,
                897800,
                924800,
                952200,
                980000,
                1008200,
                1036800,
                1065800,
                1095200,
                1125000,
                1155200,
                1185800,
                1216800,
                1248200,
                1280000,
                1312200,
                1344800,
                1377800,
                1411200,
                1445000,
                1479200,
                1513800,
                1548800,
                1584200,
                1620000,
                1656200,
                1692800,
                1729800,
                1767200,
                1805000,
                1843200,
                1881800,
                1920800,
                1960200,
                2000000
            };

            MatchPlayer matchPlayer = BattlePlayer?.MatchPlayer;

            long totalExperience = User.GetComponent<UserExperienceComponent>().Experience;
            if (IsInMatch && matchPlayer != null)
            {
                int battleExperience = matchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses;
                if (IsPremium)
                    totalExperience += battleExperience * 2;
                else
                    totalExperience += battleExperience;
            }

            int correctRank = experienceForRank.IndexOf(experienceForRank.LastOrDefault(x => x <= totalExperience)) + 1;

            if (User.GetComponent<UserRankComponent>().Rank >= correctRank) return;
            User.ChangeComponent(new UserRankComponent(correctRank));
            // todo: load rank rewards from configs (https://vignette2.wikia.nocookie.net/tanki-x/images/f/fb/Rankit.png/revision/latest?cb=20170629172052)
            ShareEntities(UserRankRewardNotificationTemplate.CreateEntity(100, 5000, correctRank));

            if (!IsInMatch || matchPlayer == null) return;
            BattlePlayer.Battle.PlayersInMap.SendEvent(new UpdateRankEvent(), User);
            int currentScoreInBattle = matchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses;
            Data.SetExperience(Data.Experience + matchPlayer.GetScoreWithPremium(currentScoreInBattle));
            matchPlayer.AlreadyAddedExperience += currentScoreInBattle;
        }

        public bool IsInBattleWith(Player player)
        {
            return IsInBattle && BattlePlayer.Battle.JoinedTankPlayers.Contains(player.BattlePlayer);
        }

        public bool IsInSquadWith(Player player)
        {
            return IsInSquad && SquadPlayer.Squad.Participants.Contains(player.SquadPlayer);
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
                Connection.QueueCommands(new EntityShareCommand(entity));
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

        public override string ToString() => $"{_EndPoint ??= Connection.Socket.RemoteEndPoint}{(ClientSession != null ? $" ({ClientSession.EntityId}{(UniqueId != null ? $", {UniqueId}" : null)})" : null)}";
        private EndPoint _EndPoint;
    }
}
