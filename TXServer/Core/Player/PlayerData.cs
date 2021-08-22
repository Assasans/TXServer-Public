using System;
using System.Collections.ObjectModel;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using TXServer.Core.Battles.Effect;
using TXServer.Core.ChatCommands;
using TXServer.Core.Configuration;
using TXServer.Core.Logging;
using TXServer.Database.Observable;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.DailyBonus;
using TXServer.ECSSystem.Components.Item.Module;
using TXServer.ECSSystem.Components.Item.Tank;
using TXServer.ECSSystem.Components.User;
using TXServer.ECSSystem.Components.User.Tutorial;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events.DailyBonus;
using TXServer.ECSSystem.Events.Item;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents;

namespace TXServer.Core
{

    public class PlayerData : ICloneable<PlayerData>
    {
        private void SetAndRaiseIfChanged<T>(string name, ref T backingField, T value)
        {
            backingField = value;
            bool success = Server.Instance.Database.UpdatePlayerData(this, name, value);

            Logger.Debug($"[User/{Username}] Update property {name}: {value} [success: {success}]");
        }

        private void RaiseChanged<T>(string name, T value)
        {
            bool success = Server.Instance.Database.UpdatePlayerData(this, name, value);

            Logger.Debug($"[User/{Username}] Update property {name}: {value} [success: {success}]");
        }

        public void InitDefault()
        {
            Email = "none";
            EmailSubscribed = false;
            Username = "Tanker";
            PasswordHash = Array.Empty<byte>();
            RememberMe = true;
            CountryCode = "EN";
            Admin = true;
            Beta = true;
            Mod = true;

            CheatSusActions = 0;

            Crystals = 1000000;
            XCrystals = 1000000;
            Experience = 0;
            GoldBonus = 5;
            PremiumExpirationDate = DateTime.MinValue;

            Fraction = null;
            FractionUserScore = 0;

            DailyBonusCycle = 0;
            DailyBonusNextReceiveDate = DateTime.UtcNow;
            DailyBonusReceivedRewards = new ObservableCollection<long>();
            DailyBonusZone = 0;

            RegistrationDate = DateTimeOffset.UtcNow;
            LastRecruitReward = null;
            RecruitRewardDay = 0;

            LastSeasonBattles = 0;
            LastSeasonLeagueId = Leagues.GlobalItems.Training.EntityId;
            LastSeasonLeagueIndex = 0;
            LastSeasonPlace = 1;
            LastSeasonLeaguePlace = 1;
            SeasonsReputation = new ObservableDictionary<int, int> { { Server.Instance.ServerData.SeasonNumber, 100 } };

            League = Leagues.GlobalItems.Training;
            LeagueChestScore = 0;
            LeagueIndex = 0;
            Reputation = 100;

            AcceptedFriendIds = new ObservableList<long>();
            IncomingFriendIds = new ObservableList<long>();
            OutgoingFriendIds = new ObservableList<long>();
            BlockedPlayerIds = new ObservableList<long>();
            ReportedPlayerIds = new ObservableList<long>();

            Punishments = new ObservableList<Punishment>();
            CompletedTutorialIds = new ObservableList<ulong>();

            Avatar = 6224;
            Avatars = new ObservableList<long> { 6224 };
            Containers = new ObservableDictionary<long, int>();
            Covers = new ObservableList<long> { -172249613 };
            Graffities = new ObservableList<long> { 1001404575 };
            Hulls = new ObservableDictionary<long, long> { { 537781597, 0 } };
            HullSkins = new ObservableList<long> { 1589207088 };
            Modules = new ObservableDictionary<long, ModuleInfo>();
            Paints = new ObservableList<long> { -20020438 };
            Shards = new ObservableDictionary<long, int>();
            Shells = new ObservableList<long>
            {
                -966935184, 807172229, 357929046, 48235025, 1067800943, 1322064226, 70311513,
                530945311, -1408603862, 139800007, 366763244
            };
            Weapons = new ObservableDictionary<long, long> { { -2005747272, 0 } };
            WeaponSkins = new ObservableList<long> { 2008385753 };

            ReceivedFractionsCompetitionReward = false;
            ReceivedReleaseReward = false;
            ShowedFractionsCompetition = false;
            RewardedLeagues = new ObservableList<long>();
        }

        [BsonConstructor]
        public PlayerData(long uniqueId)
        {
            UniqueId = uniqueId;
        }

        [BsonIgnore] public Player Player { get; set; }

        public string Username
        {
            get => _username;
            set
            {
                SetAndRaiseIfChanged(nameof(Username), ref _username, value);

                if (Player?.User is null) return;
                Player?.User.ChangeComponent(new UserUidComponent(value));
            }
        }
        [BsonId] public long UniqueId
        {
            get => _uniqueId;
            set => SetAndRaiseIfChanged(nameof(UniqueId), ref _uniqueId, value);
        }
        public string Email
        {
            get => _email;
            set
            {
                SetAndRaiseIfChanged(nameof(Email), ref _email, value);

                if (Player?.User is null) return;
                Player.User.AddOrChangeComponent(new ConfirmedUserEmailComponent(value, false));
            }
        }
        public bool EmailVerified
        {
            get => _emailVerified;
            set => SetAndRaiseIfChanged(nameof(EmailVerified), ref _emailVerified, value);
        }
        public bool EmailSubscribed
        {
            get => _emailSubscribed;
            set
            {
                SetAndRaiseIfChanged(nameof(EmailSubscribed), ref _emailSubscribed, value);

                if (Player?.User is null) return;
                Player.User.AddOrChangeComponent(new ConfirmedUserEmailComponent(Email, value));
            }
        }

        public byte[] PasswordHash
        {
            get => _passwordHash;
            set => SetAndRaiseIfChanged(nameof(PasswordHash), ref _passwordHash, value);
        }
        public string HardwareId { get => _hardwareId; set => SetAndRaiseIfChanged(nameof(HardwareId), ref _hardwareId, value); }
        public byte[] AutoLoginToken { get => _autoLoginToken; set => SetAndRaiseIfChanged(nameof(AutoLoginToken), ref _autoLoginToken, value); }

        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                SetAndRaiseIfChanged(nameof(RememberMe), ref _rememberMe, value);
                if (!value) AutoLoginToken = null;
            }
        }

        public int CheatSusActions { get => _cheatSusActions; set => SetAndRaiseIfChanged(nameof(CheatSusActions), ref _cheatSusActions, value); }

        public string CountryCode
        {
            get => _countryCode;
            set
            {
                SetAndRaiseIfChanged(nameof(CountryCode), ref _countryCode, value);

                if (Player?.User is null) return;
                Player.User.ChangeComponent(new UserCountryComponent(value));
            }
        }

        public bool Admin
        {
            get => _admin;
            set
            {
                bool wasAdmin = _admin;
                SetAndRaiseIfChanged(nameof(Admin), ref _admin, value);

                if (Player?.User is null) return;
                switch (value)
                {
                    case true when !wasAdmin:
                        Player.User.Components.Add(new UserAdminComponent());
                        break;
                    case false when wasAdmin:
                        Player.User.Components.Remove(new UserAdminComponent());
                        break;
                }
            }
        }
        public bool Beta
        {
            get => _beta;
            set
            {
                bool wasBeta = _beta;
                SetAndRaiseIfChanged(nameof(Beta), ref _beta, value);

                if (Player?.User is null) return;
                switch (value)
                {
                    case true when !wasBeta:
                        Player.User.Components.Add(new ClosedBetaQuestAchievementComponent());
                        break;
                    case false when wasBeta:
                        Player.User.Components.Remove(new ClosedBetaQuestAchievementComponent());
                        break;
                }
            }
        }
        public bool Mod { get => _mod; set => SetAndRaiseIfChanged(nameof(Mod), ref _mod, value); }

        public long Crystals
        {
            get => _crystals;
            set
            {
                SetAndRaiseIfChanged(nameof(Crystals), ref _crystals, value);

                if (Player?.User is null) return;
                Player.User.ChangeComponent(new UserMoneyComponent(value));
            }
        }
        public long XCrystals
        {
            get => _xCrystals;
            set
            {
                SetAndRaiseIfChanged(nameof(XCrystals), ref _xCrystals, value);

                if (Player?.User is null) return;
                Player.User.ChangeComponent(new UserXCrystalsComponent(value));
            }
        }

        public int CurrentBattleSeries { get; set; }
        public long Experience
        {
            get => _experience;
            set
            {
                SetAndRaiseIfChanged(nameof(Experience), ref _experience, value);

                if (Player?.User is null) return;
                Player.User.ChangeComponent(new UserExperienceComponent(value));
            }
        }

        public int GoldBonus
        {
            get => _goldBonus;
            set
            {
                SetAndRaiseIfChanged(nameof(GoldBonus), ref _goldBonus, value);

                if (Player?.User is null) return;
                Player.EntityList.SingleOrDefault(e => e.TemplateAccessor.Template is GoldBonusUserItemTemplate)
                    ?.ChangeComponent<UserItemCounterComponent>(component => component.Count = value);
            }
        }

        [BsonIgnore] public Entity Fraction { get; set; }

        public long FractionUserScore
        {
            get => _fractionUserScore;
            set
            {
                SetAndRaiseIfChanged(nameof(FractionUserScore), ref _fractionUserScore, value);

                if (Player?.User is null) return;
                Player.User.ChangeComponent<FractionUserScoreComponent>(component => component.TotalEarnedPoints = value);
            }
        }

        public int DailyBonusCycle
        {
            get => _dailyBonusCycle;
            set
            {
                SetAndRaiseIfChanged(nameof(DailyBonusCycle), ref _dailyBonusCycle, value);

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
                SetAndRaiseIfChanged(nameof(DailyBonusNextReceiveDate), ref _dailyBonusNextReceiveDate, value);

                if (Player?.User is null) return;
                Player.User.ChangeComponent<UserDailyBonusNextReceivingDateComponent>(component =>
                {
                    component.Date = value;
                    component.TotalMillisLength = (long) (value - DateTime.UtcNow).TotalMilliseconds;
                });
            }
        }
        public ObservableCollection<long> DailyBonusReceivedRewards
        {
            get => _dailyBonusReceivedRewards;
            set
            {
                SetAndRaiseIfChanged(nameof(DailyBonusReceivedRewards), ref _dailyBonusReceivedRewards, value);
            }
        }
        public int DailyBonusZone
        {
            get => _dailyBonusZone;
            set
            {
                SetAndRaiseIfChanged(nameof(DailyBonusZone), ref _dailyBonusZone, value);

                if (Player?.User is null) return;
                Player.User.ChangeComponent<UserDailyBonusZoneComponent>(component => component.ZoneNumber = value);
                Player.SendEvent(new DailyBonusZoneSwitchedEvent(), Player.User);
            }
        }

        public DateTimeOffset RegistrationDate
        {
            get => _registrationDate;
            set => SetAndRaiseIfChanged(nameof(RegistrationDate), ref _registrationDate, value);
        }
        public DateTimeOffset? LastRecruitReward
        {
            get => _lastRecruitReward;
            set => SetAndRaiseIfChanged(nameof(LastRecruitReward), ref _lastRecruitReward, value);
        }
        public int RecruitRewardDay
        {
            get => _recruitRewardDay;
            set => SetAndRaiseIfChanged(nameof(RecruitRewardDay), ref _recruitRewardDay, value);
        }

        [BsonIgnore] public Entity League { get; protected set; }
        public int LeagueIndex
        {
            get => _leagueIndex;
            protected set
            {
                SetAndRaiseIfChanged(nameof(LeagueIndex), ref _leagueIndex, value);
                League = Leagues.ByIndex[value];
            }
        }
        public int Reputation
        {
            get => _reputation;
            set
            {
                SetAndRaiseIfChanged(nameof(Reputation), ref _reputation, value);

                SeasonsReputation ??= new ObservableDictionary<int, int>();
                SeasonsReputation[Server.Instance.ServerData.SeasonNumber] = value;

                if (Player?.User is null) return;
                Player.User.ChangeComponent<UserReputationComponent>(component => component.Reputation = value);
                SetLeague(value);
            }
        }
        public long LeagueChestScore
        {
            get => _leagueChestScore;
            set
            {
                SetAndRaiseIfChanged(nameof(LeagueChestScore), ref _leagueChestScore, value);

                if (Player?.User is null) return;
                Player.User.ChangeComponent<GameplayChestScoreComponent>(component => component.Current = value);
            }
        }

        public long LastSeasonBattles
        {
            get => _lastSeasonBattles;
            set => SetAndRaiseIfChanged(nameof(LastSeasonBattles), ref _lastSeasonBattles, value);
        }
        public long LastSeasonLeagueId
        {
            get => _lastSeasonLeagueId;
            set => SetAndRaiseIfChanged(nameof(LastSeasonLeagueId), ref _lastSeasonLeagueId, value);
        }
        public int LastSeasonLeagueIndex
        {
            get => _lastSeasonLeagueIndex;
            set => SetAndRaiseIfChanged(nameof(LastSeasonLeagueIndex), ref _lastSeasonLeagueIndex, value);
        }
        public int LastSeasonPlace
        {
            get => _lastSeasonPlace;
            set => SetAndRaiseIfChanged(nameof(LastSeasonPlace), ref _lastSeasonPlace, value);
        }
        public int LastSeasonLeaguePlace
        {
            get => _lastSeasonLeaguePlace;
            set => SetAndRaiseIfChanged(nameof(LastSeasonLeaguePlace), ref _lastSeasonLeaguePlace, value);
        }

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)] public ObservableDictionary<int, int> SeasonsReputation { get; protected set; }

        public DateTime PremiumExpirationDate
        {
            get => _premiumExpirationDate;
            set
            {
                SetAndRaiseIfChanged(nameof(PremiumExpirationDate), ref _premiumExpirationDate, value);

                if (Player?.User is null) return;
                Player.User.AddOrChangeComponent(new PremiumAccountBoostComponent(PremiumExpirationDate));
            }
        }

        [BsonIgnore] public ObservableList<Entity> Presets { get; } = new();

        public ObservableList<long> AcceptedFriendIds { get; private set; }
        public ObservableList<long> IncomingFriendIds { get; private set; }
        public ObservableList<long> OutgoingFriendIds { get; private set; }
        public ObservableList<long> BlockedPlayerIds { get; private set; }
        public ObservableList<long> ReportedPlayerIds { get; private set; }

        public ObservableList<ulong> CompletedTutorialIds { get; set; }

        [BsonIgnore] public ObservableList<Punishment> Punishments { get; protected set; } = new ObservableList<Punishment>(); // TODO(Assasans): Store in DB

        public ObservableList<long> Avatars { get; protected set; }
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)] public ObservableDictionary<long, int> Containers { get; protected set; }
        public ObservableList<long> Covers { get; protected set; }

        public long Avatar
        {
            get => _avatar;
            set
            {
                SetAndRaiseIfChanged(nameof(Avatar), ref _avatar, value);

                if (Player?.User is null) return;
                string configPath = Player.GetEntityById(value).TemplateAccessor.ConfigPath;
                string avatarId = Config.GetComponent<AvatarItemComponent>(configPath).Id;

                Player.User.ChangeComponent<UserAvatarComponent>(component => component.Id = avatarId);
            }
        }
        public ObservableList<long> Graffities { get; protected set; }
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)] public ObservableDictionary<long, long> Hulls
        {
            get => _hulls;
            protected set
            {
                _hulls = value;
                _hulls.Changed += (_, _) => RaiseChanged(nameof(Hulls), _hulls);
            }
        }
        public ObservableList<long> HullSkins
        {
            get => _hullSkins;
            protected set
            {
                _hullSkins = value;
                _hullSkins.Changed += (_, _) => RaiseChanged(nameof(HullSkins), _hullSkins);
            }
        }
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)] public ObservableDictionary<long, ModuleInfo> Modules
        {
            get => _modules;
            protected set
            {
                _modules = value;
                _modules.Changed += (_, _) => RaiseChanged(nameof(Modules), _modules);
            }
        }
        public ObservableList<long> Paints
        {
            get => _paints;
            protected set
            {
                _paints = value;
                _paints.Changed += (_, _) => RaiseChanged(nameof(Paints), _paints);
            }
        }
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)] public ObservableDictionary<long, int> Shards
        {
            get => _shards;
            protected set
            {
                _shards = value;
                _shards.Changed += (_, _) => RaiseChanged(nameof(Shards), _shards);
            }
        }
        public ObservableList<long> Shells
        {
            get => _shells;
            protected set
            {
                _shells = value;
                _shells.Changed += (_, _) => RaiseChanged(nameof(Shells), _shells);
            }
        }
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)] public ObservableDictionary<long, long> Weapons
        {
            get => _weapons;
            protected set
            {
                _weapons = value;
                _weapons.Changed += (_, _) => RaiseChanged(nameof(Weapons), _weapons);
            }
        }
        public ObservableList<long> WeaponSkins
        {
            get => _weaponSkins;
            protected set
            {
                _weaponSkins = value;
                _weaponSkins.Changed += (_, _) => RaiseChanged(nameof(WeaponSkins), _weaponSkins);
            }
        }

        public bool ReceivedFractionsCompetitionReward
        {
            get => _receivedFractionsCompetitionReward;
            set => SetAndRaiseIfChanged(nameof(ReceivedFractionsCompetitionReward), ref _receivedFractionsCompetitionReward, value);
        }
        public bool ReceivedReleaseReward
        {
            get => _receivedReleaseReward;
            set => SetAndRaiseIfChanged(nameof(ReceivedReleaseReward), ref _receivedReleaseReward, value);
        }
        public bool ReceivedLastSeasonReward
        {
            get => _receivedLastSeasonReward;
            set => SetAndRaiseIfChanged(nameof(ReceivedLastSeasonReward), ref _receivedLastSeasonReward, value);
        }
        public bool ShowedFractionsCompetition
        {
            get => _showedFractionsCompetition;
            set => SetAndRaiseIfChanged(nameof(ShowedFractionsCompetition), ref _showedFractionsCompetition, value);
        }
        public ObservableList<long> RewardedLeagues
        {
            get => _rewardedLeagues;
            private set
            {
                _rewardedLeagues = value;
                _rewardedLeagues.Changed += (_, _) => RaiseChanged(nameof(RewardedLeagues), _rewardedLeagues);
            }
        }

        public bool OwnsMarketItem(Entity marketItem) => Avatars
            .Concat(Covers).Concat(Graffities).Concat(Hulls.Keys)
            .Concat(HullSkins).Concat(Paints).Concat(Shells).Concat(Weapons.Keys).Concat(WeaponSkins)
            .Contains(marketItem.EntityId);

        public bool IsPremium => PremiumExpirationDate > DateTime.UtcNow;

        private void SetLeague(int reputation)
        {
            (League, LeagueIndex) = reputation switch
            {
                <= 139 => (Leagues.GlobalItems.Training, 0),
                >= 140 and <= 999 => (Leagues.GlobalItems.Bronze, 1),
                >= 1000 and <= 2999 => (Leagues.GlobalItems.Silver, 2),
                >= 3000 and <= 4499 => (Leagues.GlobalItems.Gold, 3),
                >= 4500 => (Leagues.GlobalItems.Master, 4)
            };
            Player.User.TryRemoveComponent<LeagueGroupComponent>();
            Player.User.AddComponent(League.GetComponent<LeagueGroupComponent>());
        }

        public void SetExperience(long value, bool rankUpCheck = true)
        {
            Experience = value;
            if (rankUpCheck) Player.CheckRankUp();
        }

        public void RenewPremium(TimeSpan additionalPremiumTime)
        {
            if (IsPremium) PremiumExpirationDate += additionalPremiumTime;
            else PremiumExpirationDate = DateTime.UtcNow + additionalPremiumTime;
        }

        public void AddDailyBonusReward(long code)
        {
            DailyBonusReceivedRewards.Add(code);

            RaiseChanged(nameof(DailyBonusReceivedRewards), DailyBonusReceivedRewards);

            Player.User.ChangeComponent<UserDailyBonusReceivedRewardsComponent>(component => component.ReceivedRewards.Add(code));
        }
        public void ResetDailyBonusRewards()
        {
            DailyBonusReceivedRewards.Clear();

            RaiseChanged(nameof(DailyBonusReceivedRewards), DailyBonusReceivedRewards);

            Player.User.ChangeComponent<UserDailyBonusReceivedRewardsComponent>(component => component.ReceivedRewards.Clear());
        }

        public void AddIncomingFriend(long userId)
        {
            IncomingFriendIds.Add(userId);

            RaiseChanged(nameof(IncomingFriendIds), IncomingFriendIds);
        }
        public void AddOutgoingFriend(long userId)
        {
            OutgoingFriendIds.Add(userId);

            RaiseChanged(nameof(OutgoingFriendIds), OutgoingFriendIds);
        }
        public void AddAcceptedFriend(long userId)
        {
            IncomingFriendIds.Remove(userId);
            OutgoingFriendIds.Remove(userId);
            AcceptedFriendIds.Add(userId);

            RaiseChanged(nameof(IncomingFriendIds), IncomingFriendIds);
            RaiseChanged(nameof(OutgoingFriendIds), OutgoingFriendIds);
            RaiseChanged(nameof(AcceptedFriendIds), AcceptedFriendIds);
        }
        public void RemoveFriend(long userId)
        {
            IncomingFriendIds.Remove(userId);
            OutgoingFriendIds.Remove(userId);
            AcceptedFriendIds.Remove(userId);

            RaiseChanged(nameof(IncomingFriendIds), IncomingFriendIds);
            RaiseChanged(nameof(OutgoingFriendIds), OutgoingFriendIds);
            RaiseChanged(nameof(AcceptedFriendIds), AcceptedFriendIds);
        }

        public void ChangeBlockedPlayer(long userId)
        {
            if (BlockedPlayerIds.Contains(userId)) BlockedPlayerIds.Remove(userId);
            else BlockedPlayerIds.Add(userId);

            RaiseChanged(nameof(BlockedPlayerIds), BlockedPlayerIds);
        }

        public void AddReportedPlayer(long userId)
        {
            ReportedPlayerIds.Add(userId);

            RaiseChanged(nameof(ReportedPlayerIds), ReportedPlayerIds);
        }

        public void AddCompletedTutorial(ulong tutorialId)
        {
            CompletedTutorialIds.Add(tutorialId);

            RaiseChanged(nameof(CompletedTutorialIds), CompletedTutorialIds);

            if(Player?.User is null) return;
            Player.User.ChangeComponent<TutorialCompleteIdsComponent>(component => component.CompletedIds.Add(tutorialId));
        }

        public void AddHullXp(int xp, Entity hull)
        {
            Hulls[hull.EntityId] += xp;

            RaiseChanged(nameof(Hulls), Hulls);

            ResourceManager
                .GetUserItem(Player, hull)
                .ChangeComponent<ExperienceItemComponent>(component => component.Experience = Hulls[hull.EntityId]);
        }
        public void AddWeaponXp(int xp, Entity weapon)
        {
            Weapons[weapon.EntityId] += xp;

            RaiseChanged(nameof(Weapons), Weapons);

            ResourceManager
                .GetUserItem(Player, weapon)
                .ChangeComponent<ExperienceItemComponent>(component => component.Experience = Weapons[weapon.EntityId]);
        }

        public void UpgradeModule(Entity marketItem, bool assemble = false)
        {
            if (!Modules.ContainsKey(marketItem.GetComponent<ParentGroupComponent>().Key)) return;

            long id = marketItem.GetComponent<ParentGroupComponent>().Key;
            ModuleInfo info = Modules[id];

            Entity moduleUserItem = ResourceManager.GetModuleUserItem(Player, id);
            Entity cardUserItem = Player.EntityList.Single(e => e.TemplateAccessor.Template is ModuleCardUserItemTemplate && e.GetComponent<ParentGroupComponent>().Key == id);

            if (!assemble) info.Level++;
            info.Cards -= moduleUserItem.GetComponent<ModuleCardsCompositionComponent>().AllPrices[info.Level].Cards;
            Modules[id] = info;

            RaiseChanged(nameof(Modules), Modules);

            moduleUserItem.ChangeComponent<ModuleUpgradeLevelComponent>(component => component.Level = info.Level);
            cardUserItem.ChangeComponent<UserItemCounterComponent>(component => component.Count = info.Cards);

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

        public bool Save()
        {
            return Server.Instance.Database.SavePlayerData(this);
            // return false;
        }

        public PlayerData Clone()
        {
            return (PlayerData) MemberwiseClone();
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
        private long _fractionUserScore;
        private int _goldBonus;
        private string _countryCode;
        private bool _emailSubscribed;
        private string _email;
        private bool _admin;
        private bool _beta;
        private long _experience;
        private bool _rememberMe;
        private DateTime _premiumExpirationDate;
        private byte[] _autoLoginToken;
        private string _hardwareId;
        private byte[] _passwordHash;
        private bool _mod;
        private ObservableCollection<long> _dailyBonusReceivedRewards;
        private DateTimeOffset _registrationDate;
        private DateTimeOffset? _lastRecruitReward;
        private int _recruitRewardDay;
        private int _leagueIndex;
        private long _lastSeasonBattles;
        private long _lastSeasonLeagueId;
        private int _lastSeasonLeagueIndex;
        private int _lastSeasonPlace;
        private int _lastSeasonLeaguePlace;
        private bool _receivedFractionsCompetitionReward;
        private bool _receivedReleaseReward;
        private bool _receivedLastSeasonReward;
        private bool _showedFractionsCompetition;
        private int _cheatSusActions;
        private bool _emailVerified;
        private long _uniqueId;
        private ObservableDictionary<long, long> _hulls;
        private ObservableList<long> _hullSkins;
        private ObservableDictionary<long, ModuleInfo> _modules;
        private ObservableList<long> _weaponSkins;
        private ObservableDictionary<long, long> _weapons;
        private ObservableList<long> _shells;
        private ObservableDictionary<long, int> _shards;
        private ObservableList<long> _paints;
        private ObservableList<long> _rewardedLeagues;
    }
}
