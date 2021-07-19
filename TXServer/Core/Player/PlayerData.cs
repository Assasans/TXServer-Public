using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TXServer.Core.Commands;
using TXServer.Core.Data.Database;
using TXDatabase.NetworkEvents.PlayerAuth;
using TXDatabase.NetworkEvents.PlayerSettings;
using TXServer.Core.Configuration;
using TXServer.Core.Logging;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.DailyBonus;
using TXServer.ECSSystem.Components.Item.Module;
using TXServer.ECSSystem.Components.Item.Tank;
using TXServer.ECSSystem.Components.User;
using TXServer.ECSSystem.Components.User.Tutorial;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.DailyBonus;
using TXServer.ECSSystem.Events.Item;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents;

namespace TXServer.Core
{
    public abstract class PlayerData : ICloneable
    {
        protected PlayerData Original { get; set; }
        public Player Player { get; set; }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;

                if (Player?.User is null) return;
                Player.User.ChangeComponent(new UserUidComponent(value));
                if (Server.DatabaseNetwork.IsReady)
                    Server.DatabaseNetwork.Socket.emit(new SetUsername
                    {
                        uid = UniqueId, username = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(value)
                    });
            }
        }
        public long UniqueId { get; }
        public string Email { get; protected set; }
        protected bool EmailVerified { get; set; }
        public bool Subscribed { get; protected set; }

        public string HashedPassword { get; set; }
        public string HardwareId { get; set; }
        public string AutoLoginToken { get; protected set; }
        public bool RememberMe { get; set; }

        public string CountryCode { get; protected set; }

        public long Avatar
        {
            get => _avatar;
            set
            {
                _avatar = value;
                if (Player?.User is null) return;

                // todo: database set
                string configPath = Player.EntityList.Single(e => e.EntityId == value).TemplateAccessor.ConfigPath;
                string avatarId = Config.GetComponent<AvatarItemComponent>(configPath).Id;
                Player.User.ChangeComponent<UserAvatarComponent>(component => component.Id = avatarId);
            }
        }

        public bool Admin { get; set; }
        public bool Beta { get; protected set; }
        public bool Mod { get; set; }

        public long Crystals
        {
            get => _crystals;
            set
            {
                _crystals = value;
                if (Player?.User is null) return;

                // todo: database set
                Player.User.ChangeComponent(new UserMoneyComponent(value));
            }
        }
        public long XCrystals
        {
            get => _xCrystals;
            set
            {
                _xCrystals = value;
                if (Player?.User is null) return;

                // todo: database set
                Player.User.ChangeComponent(new UserXCrystalsComponent(value));
            }
        }

        public int CurrentBattleSeries { get; set; }
        public long Experience { get; protected set; }
        public int GoldBoxes { get; private set; } = 5;

        public int DailyBonusCycle
        {
            get => _dailyBonusCycle;
            set
            {
                _dailyBonusCycle = value;
                if (Player?.User is null) return;

                Player.User.ChangeComponent<UserDailyBonusCycleComponent>(component => component.CycleNumber = value);
                Player.SendEvent(new DailyBonusCycleSwitchedEvent(), Player.User);
            }
        }
        public DateTime DailyBonusNextReceiveDate
        {
            get => _dailyBonusNextReceiveDate;
            set
            {
                _dailyBonusNextReceiveDate = value;
                if (Player?.User is null) return;

                Player.User.ChangeComponent<UserDailyBonusNextReceivingDateComponent>(component =>
                {
                    component.Date = value;
                    component.TotalMillisLength = (long) (value - DateTime.UtcNow).TotalMilliseconds;
                });
            }
        }
        protected List<long> DailyBonusReceivedRewards { get; set; }
        public int DailyBonusZone
        {
            get => _dailyBonusZone;
            set
            {
                _dailyBonusZone = value;
                if (Player?.User is null) return;

                Player.User.ChangeComponent<UserDailyBonusZoneComponent>(component => component.ZoneNumber = value);
                Player.SendEvent(new DailyBonusZoneSwitchedEvent(), Player.User);
            }
        }

        public DateTimeOffset RegistrationDate { get; set; }
        public DateTimeOffset? LastRecruitReward { get; set; }
        public int RecruitRewardDay { get; set; }

        public Entity League { get; protected set; }
        public int Reputation
        {
            get => _reputation;
            set
            {
                _reputation = value;

                if (Player?.User is null) return;
                //todo: save in db
                Player.User.ChangeComponent<UserReputationComponent>(component => component.Reputation = value);
                SetLeague(value);
            }
        }
        public long LeagueChestScore
        {
            get => _leagueChestScore;
            set
            {
                _leagueChestScore = value;

                if (Player?.User is null) return;
                //todo: save in db
                Player.User.ChangeComponent<GameplayChestScoreComponent>(component => component.Current = value);
            }
        }

        public DateTime PremiumExpirationDate { get; protected set; }

        public List<Entity> Presets { get; } = new();

        public List<long> AcceptedFriendIds { get; protected set; }
        public List<long> IncomingFriendIds { get; protected set; }
        public List<long> OutgoingFriendIds { get; protected set; }
        public List<long> BlockedPlayerIds { get; protected set; }
        public List<long> ReportedPlayerIds { get; protected set; }

        public List<ulong> CompletedTutorialIds { get; set; }
        public List<ChatCommands.Punishment> Punishments { get; protected set; }

        public List<long> Avatars { get; protected set; }
        public Dictionary<long, int> Containers { get; protected set; }
        public List<long> Covers { get; protected set; }
        public List<long> Graffities { get; protected set; }
        public Dictionary<long, long> Hulls { get; protected set; }
        public List<long> HullSkins { get; protected set; }
        public Dictionary<long, (int, int)> Modules { get; protected set; }
        public List<long> Paints { get; protected set; }
        public Dictionary<long, int> Shards { get; protected set; }
        public List<long> Shells { get; protected set; }
        public Dictionary<long, long> Weapons { get; protected set; }
        public List<long> WeaponSkins { get; protected set; }


        public PlayerData(string uid)
        {
            Username = uid;
        }

        public PlayerData(long uniqueId)
            => UniqueId = uniqueId;

        public bool OwnsMarketItem(Entity marketItem) => Avatars.Concat(Covers).Concat(Graffities).Concat(Hulls.Keys)
            .Concat(HullSkins).Concat(Paints).Concat(Shells).Concat(Weapons.Keys).Concat(WeaponSkins)
            .Contains(marketItem.EntityId);
        public bool IsPremium => PremiumExpirationDate > DateTime.UtcNow;

        private void SetLeague(int reputation)
        {
            League = reputation switch
            {
                <= 139 => Leagues.GlobalItems.Training,
                >= 140 and <= 999 => Leagues.GlobalItems.Bronze,
                >= 1000 and <= 2999 => Leagues.GlobalItems.Silver,
                >= 3000 and <= 4499 => Leagues.GlobalItems.Gold,
                >= 4500 => Leagues.GlobalItems.Master
            };
            Player.User.TryRemoveComponent<LeagueGroupComponent>();
            Player.User.AddComponent(League.GetComponent<LeagueGroupComponent>());
        }

        public ConfirmedUserEmailComponent SetEmail(string email)
        {
            ConfirmedUserEmailComponent component = SetValue<ConfirmedUserEmailComponent>(email);
            Email = email;
            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetEmail
                    {uid = UniqueId, email = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(email)});
            return component;
        }

        public ConfirmedUserEmailComponent SetSubscribed(bool subscribed)
        {
            ConfirmedUserEmailComponent component = SetValue<ConfirmedUserEmailComponent>(subscribed);
            Subscribed = subscribed;
            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetSubscribed { uid = UniqueId, state = subscribed });
            return component;
        }

        public void SetHashedPassword(string hashedPassword)
        {
            HashedPassword = hashedPassword;
            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetHashedPassword
                {
                    uid = UniqueId,
                    hashedPassword = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(hashedPassword)
                });
        }

        public void SetCountryCode(string countryCode)
        {
            CountryCode = countryCode;
            Player.User.ChangeComponent(new UserCountryComponent(countryCode));
            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetCountryCode
                {
                    uid = UniqueId,
                    countryCode = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(countryCode)
                });
        }

        public void SetAdmin(bool admin)
        {
            if (admin && !Admin)
                Player.User.Components.Add(new UserAdminComponent());
            else if (!admin && Admin)
                Player.User.Components.Remove(new UserAdminComponent());
            Admin = admin;
        }
        public void SetBeta(bool beta)
        {
            if (beta && !Beta)
                Player.User.Components.Add(new ClosedBetaQuestAchievementComponent());
            else if (!beta && Beta)
                Player.User.Components.Remove(new ClosedBetaQuestAchievementComponent());
            Beta = beta;
        }

        public void SetExperience(long value, bool rankUpCheck = true)
        {
            Experience = value;
            Player.User.ChangeComponent(new UserExperienceComponent(value));
            if (rankUpCheck) Player.CheckRankUp();
        }
        public void SetGoldBoxes(int value)
        {
            GoldBoxes = value;

            // todo: better solution for this
            var goldBonus = Player.EntityList.Single(i => i.TemplateAccessor.Template is GoldBonusUserItemTemplate);
            goldBonus.ChangeComponent<UserItemCounterComponent>(component => component.Count = value);
        }

        public void SetAutoLogin(bool value)
        {
            RememberMe = value;
            if (!RememberMe) AutoLoginToken = "";
        }
        public void RenewPremium(TimeSpan additionalPremiumTime)
        {
            if (IsPremium) PremiumExpirationDate += additionalPremiumTime;
            else PremiumExpirationDate = DateTime.UtcNow + additionalPremiumTime;

            PremiumAccountBoostComponent component = new() { EndDate = PremiumExpirationDate };

            if (Player.User.GetComponent<PremiumAccountBoostComponent>() == null)
                Player.User.AddComponent(component);
            else
                Player.User.ChangeComponent(component);

            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetPremiumExpiration() { uid = UniqueId, expiration = PremiumExpirationDate.Ticks });
        }

        public void AddDailyBonusReward(long code)
        {
            DailyBonusReceivedRewards.Add(code);
            Player.User.ChangeComponent<UserDailyBonusReceivedRewardsComponent>(component =>
                            component.ReceivedRewards.Add(code));
        }
        public void ResetDailyBonusRewards()
        {
            DailyBonusReceivedRewards.Clear();
            Player.User.ChangeComponent<UserDailyBonusReceivedRewardsComponent>(component =>
                component.ReceivedRewards.Clear());
        }

        public void AddIncomingFriend(long userId)
        {
            IncomingFriendIds.Add(userId);
        }
        public void AddOutgoingFriend(long userId)
        {
            OutgoingFriendIds.Add(userId);
        }
        public void AddAcceptedFriend(long userId)
        {
            IncomingFriendIds.Remove(userId);
            OutgoingFriendIds.Remove(userId);
            AcceptedFriendIds.Add(userId);
        }
        public void RemoveFriend(long userId)
        {
            IncomingFriendIds.Remove(userId);
            OutgoingFriendIds.Remove(userId);
            AcceptedFriendIds.Remove(userId);
        }
        public void ChangeBlockedPlayer(long userId)
        {
            if (BlockedPlayerIds.Contains(userId))
                BlockedPlayerIds.Remove(userId);
            else
                BlockedPlayerIds.Add(userId);
        }
        public void AddReportedPlayer(long userId)
        {
            ReportedPlayerIds.Add(userId);
        }

        public void AddCompletedTutorial(ulong tutorialId)
        {
            Player.User.ChangeComponent<TutorialCompleteIdsComponent>(component =>
                component.CompletedIds.Add(tutorialId));
            CompletedTutorialIds.Add(tutorialId);
        }

        public void AddHullXp(int xp, Entity hull)
        {
            Hulls[hull.EntityId] += xp;
            ResourceManager.GetUserItem(Player, hull)
                .ChangeComponent<ExperienceItemComponent>(component => component.Experience = Hulls[hull.EntityId]);
            // todo: save to database
        }
        public void AddWeaponXp(int xp, Entity weapon)
        {
            Weapons[weapon.EntityId] += xp;
            ResourceManager.GetUserItem(Player, weapon).ChangeComponent<ExperienceItemComponent>(
                component => component.Experience = Weapons[weapon.EntityId]);
            // todo: save to database
        }

        public void UpgradeModule(Entity marketItem, bool assemble = false)
        {
            if (!Modules.ContainsKey(marketItem.GetComponent<ParentGroupComponent>().Key)) return;

            long id = marketItem.GetComponent<ParentGroupComponent>().Key;
            (int level, int cards) infos = Modules[id];

            Entity moduleUserItem = ResourceManager.GetModuleUserItem(Player, id);
            Entity cardUserItem = Player.EntityList.Single(e => e.TemplateAccessor.Template is
                ModuleCardUserItemTemplate && e.GetComponent<ParentGroupComponent>().Key == id);

            if (!assemble) infos.level++;
            infos.cards -= moduleUserItem.GetComponent<ModuleCardsCompositionComponent>().AllPrices[infos.level].Cards;
            Modules[id] = infos;

            moduleUserItem.ChangeComponent<ModuleUpgradeLevelComponent>(component => component.Level = infos.level);
            cardUserItem.ChangeComponent<UserItemCounterComponent>(component => component.Count = infos.cards);

            if (!moduleUserItem.HasComponent<UserGroupComponent>())
            {
                // research module
                moduleUserItem.AddComponent(new UserGroupComponent(Player.User.EntityId));
                Player.SendEvent(new ModuleAssembledEvent(), moduleUserItem);
                return;
            }

            // upgrade module
            Player.SendEvent(new ModuleUpgradedEvent(), moduleUserItem);
        }

        private T SetValue<T>(object value) where T : Component
        {
            T component = Player.User.GetComponent<T>();
            if (component == null)
            {
                component = Activator.CreateInstance<T>();
                Player.User.Components.Add(component);
            }

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                if (info.PropertyType != value.GetType()) continue;
                Logger.Debug($"{Player}: Updated user value \"{info.Name}\"");
                info.SetValue(component, value);
            }

            return component;
        }


        public abstract PlayerData From(object dataReader);

        public bool Save()
        {
            return Player.Server.Database.SavePlayerData(this);
        }

        public void Apply()
        {
            Entity user = Player.User;
            List<ICommand> commands = new List<ICommand>();

            foreach (var change in RawChanges(Original))
            {
                FieldInfo info = typeof(PlayerData).GetField(change.Key);
                LinkedComponent link = info.GetCustomAttribute(typeof(LinkedComponent)) as LinkedComponent;

                if (link != null)
                {
                    Component component;
                    Player.User.Components.TryGetValue(link.instance, out component);

                    bool isNew = info.GetValue(Original) == null;

                    if (component != null)
                    {
                        commands.Add(isNew ?
                            (ICommand) new ComponentChangeCommand(user, component) :
                            new ComponentAddCommand(user, component));
                        continue;
                    }
                    commands.Add(isNew ?
                        (ICommand) new ComponentAddCommand(user, component) :
                        new ComponentRemoveCommand(user, component.GetType())
                    );
                }
            }
        }

        protected IDictionary<string, object> RawChanges(PlayerData originalData)
        {
            IDictionary<string, object> changes = new Dictionary<string, object>();

            foreach (var field in typeof(PlayerData).GetFields(BindingFlags.GetField))
            {
                var value = field.GetValue(this);
                if (value.Equals(field.GetValue(originalData))) continue;
                changes.Add(field.Name, value);
            }

            return changes;
        }

        protected void SetValue(string field, object value)
        {
            GetType().GetField(field).SetValue(this, value);
        }

        public object Clone()
        {
            var clone = (PlayerData)MemberwiseClone();
            clone.Original = null;

            return clone;
        }


        private string _username;

        private int _reputation;
        private long _leagueChestScore;

        private long _crystals;
        private long _xCrystals;

        private long _avatar;

        private int _dailyBonusCycle;
        private DateTime _dailyBonusNextReceiveDate;
        private int _dailyBonusZone;
    }
}
