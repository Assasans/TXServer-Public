using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TXServer.Core.Battles.Effect;
using TXServer.Core.ChatCommands;
using TXServer.Core.Configuration;
using TXServer.Core.Logging;
using TXServer.Database.Entity;
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
using GlobalEntities = TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core
{
    public class DailyBonusReward : PlayerData.IEntity
    {
        public static DailyBonusReward Create(PlayerData player, long entityId)
        {
            return new DailyBonusReward()
            {
                Player = player,
                PlayerId = player.UniqueId,
                EntityId = entityId
            };
        }

        public long PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        public virtual PlayerData Player { get; set; }

        public long EntityId { get; set; }
    }

    public class PlayerCompletedTutorial : PlayerData.IEntity
    {
        public static PlayerCompletedTutorial Create(PlayerData player, long entityId)
        {
            return new PlayerCompletedTutorial()
            {
                Player = player,
                PlayerId = player.UniqueId,
                EntityId = entityId
            };
        }

        public long PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        public virtual PlayerData Player { get; set; }

        public long EntityId { get; set; }
    }

    public static class DatabaseEntityExtensions
    {
        public static TEntity GetById<TEntity>(this IEnumerable<TEntity> entities, long entityId) where TEntity : PlayerData.IEntity
        {
            return entities.SingleOrDefault(entity => entity.EntityId == entityId);
        }

        public static bool ContainsId<TEntity>(this IEnumerable<TEntity> entities, long entityId) where TEntity : PlayerData.IEntity
        {
            return entities.Any(entity => entity.EntityId == entityId);
        }

        public static IEnumerable<long> ToIds<TEntity>(this IEnumerable<TEntity> entities) where TEntity : PlayerData.IEntity
        {
            return entities.Select(entity => entity.EntityId);
        }

        public static bool TryGetById<TEntity>(this IEnumerable<TEntity> entities, long entityId, [MaybeNullWhen(false)] out TEntity property)
            where TEntity : PlayerData.IEntity
        {
            TEntity entity = entities.GetById(entityId);
            if (entity == null)
            {
                property = default;
                return false;
            }

            property = entity;
            return true;
        }

        public static bool TryGetById<TEntity, TProperty>(this IEnumerable<TEntity> entities, long entityId, Func<TEntity, TProperty> resolver, [MaybeNullWhen(false)] out TProperty property)
            where TEntity : PlayerData.IEntity
        {
            TEntity entity = entities.GetById(entityId);
            if (entity == null)
            {
                property = default;
                return false;
            }

            property = resolver(entity);
            return true;
        }
    }

    public static class PlayerEquipmentExtensions
    {
        public static TEntity GetOwnedById<TEntity>(this IEnumerable<TEntity> entities, long entityId) where TEntity : PlayerData.IEntity, PlayerData.IPlayerEquipment
        {
            return entities.SingleOrDefault(entity => entity.EntityId == entityId && entity.IsOwned);
        }

        public static bool ContainsOwnedId<TEntity>(this IEnumerable<TEntity> entities, long entityId) where TEntity : PlayerData.IEntity, PlayerData.IPlayerEquipment
        {
            return entities.Any(entity => entity.EntityId == entityId && entity.IsOwned);
        }

        public static IEnumerable<long> ToOwnedIds<TEntity>(this IEnumerable<TEntity> entities) where TEntity : PlayerData.IEntity, PlayerData.IPlayerEquipment
        {
            return entities.Where(entity => entity.IsOwned).Select(entity => entity.EntityId);
        }
    }

    public static class PlayerSeasonReputationExtensions
    {
        public static TEntity GetBySeason<TEntity>(this IEnumerable<TEntity> entities, long season) where TEntity : PlayerData.PlayerSeasonReputation
        {
            return entities.SingleOrDefault(entity => entity.Season == season);
        }

        public static bool ContainsSeason<TEntity>(this IEnumerable<TEntity> entities, long season) where TEntity : PlayerData.PlayerSeasonReputation
        {
            return entities.Any(entity => entity.Season == season);
        }

        public static bool TryGetBySeason<TEntity>(this IEnumerable<TEntity> entities, long season, [MaybeNullWhen(false)] out TEntity property)
            where TEntity : PlayerData.PlayerSeasonReputation
        {
            TEntity entity = entities.GetBySeason(season);
            if (entity == null)
            {
                property = default;
                return false;
            }

            property = entity;
            return true;
        }

        public static bool TryGetBySeason<TEntity, TProperty>(this IEnumerable<TEntity> entities, long season, Func<TEntity, TProperty> resolver, [MaybeNullWhen(false)] out TProperty property)
            where TEntity : PlayerData.PlayerSeasonReputation
        {
            TEntity entity = entities.GetBySeason(season);
            if (entity == null)
            {
                property = default;
                return false;
            }

            property = resolver(entity);
            return true;
        }
    }

    public static class PlayerPresetEquipmentExtensions
    {
        // IHullEquipment

        public static TEntity GetByHull<TEntity>(this IEnumerable<TEntity> entities, long hullId) where TEntity : PlayerData.IEntity, IHullEquipment
        {
            return entities.SingleOrDefault(entity => entity.HullId == hullId);
        }

        public static bool ContainsHull<TEntity>(this IEnumerable<TEntity> entities, long hullId) where TEntity : PlayerData.IEntity, IHullEquipment
        {
            return entities.Any(entity => entity.HullId == hullId);
        }

        public static bool TryGetByHull<TEntity>(this IEnumerable<TEntity> entities, long hullId, [MaybeNullWhen(false)] out TEntity property)
            where TEntity : PlayerData.IEntity, IHullEquipment
        {
            TEntity entity = entities.GetByHull(hullId);
            if (entity == null)
            {
                property = default;
                return false;
            }

            property = entity;
            return true;
        }

        public static bool TryGetByHull<TEntity, TProperty>(this IEnumerable<TEntity> entities, long hullId, Func<TEntity, TProperty> resolver, [MaybeNullWhen(false)] out TProperty property)
            where TEntity : PlayerData.IEntity, IHullEquipment
        {
            TEntity entity = entities.GetByHull(hullId);
            if (entity == null)
            {
                property = default;
                return false;
            }

            property = resolver(entity);
            return true;
        }

        // IWeaponEquipment

        public static TEntity GetByWeapon<TEntity>(this IEnumerable<TEntity> entities, long weaponId) where TEntity : PlayerData.IEntity, IWeaponEquipment
        {
            return entities.SingleOrDefault(entity => entity.WeaponId == weaponId);
        }

        public static bool ContainsWeapon<TEntity>(this IEnumerable<TEntity> entities, long weaponId) where TEntity : PlayerData.IEntity, IWeaponEquipment
        {
            return entities.Any(entity => entity.WeaponId == weaponId);
        }

        public static bool TryGetByWeapon<TEntity>(this IEnumerable<TEntity> entities, long weaponId, [MaybeNullWhen(false)] out TEntity property)
            where TEntity : PlayerData.IEntity, IWeaponEquipment
        {
            TEntity entity = entities.GetByWeapon(weaponId);
            if (entity == null)
            {
                property = default;
                return false;
            }

            property = entity;
            return true;
        }

        public static bool TryGetByWeapon<TEntity, TProperty>(this IEnumerable<TEntity> entities, long hullId, Func<TEntity, TProperty> resolver, [MaybeNullWhen(false)] out TProperty property)
            where TEntity : PlayerData.IEntity, IWeaponEquipment
        {
            TEntity entity = entities.GetByWeapon(hullId);
            if (entity == null)
            {
                property = default;
                return false;
            }

            property = resolver(entity);
            return true;
        }
    }

    public static class PlayerRelationExtensions
    {
        public static TEntity GetById<TEntity>(this IEnumerable<TEntity> entities, long relationId, long targetId, PlayerData.PlayerRelation.RelationType? type = null) where TEntity : PlayerData.PlayerRelation
        {
            return entities.SingleOrDefault(entity =>
            {
                if (entity.RelationId != relationId || entity.TargetId != targetId) return false;
                if (type != null && entity.Type != type) return false;
                return true;
            });
        }

        public static List<TEntity> GetAllById<TEntity>(this IEnumerable<TEntity> entities, long targetId, PlayerData.PlayerRelation.RelationType? type = null) where TEntity : PlayerData.PlayerRelation
        {
            return entities.Where(entity =>
            {
                if (entity.TargetId != targetId) return false;
                if (type != null && entity.Type != type) return false;
                return true;
            }).ToList();
        }

        public static List<TEntity> FilterType<TEntity>(this IEnumerable<TEntity> entities, PlayerData.PlayerRelation.RelationType type) where TEntity : PlayerData.PlayerRelation
        {
            return entities.Where(entity => entity.Type == type).ToList();
        }

        public static bool ContainsId<TEntity>(this IEnumerable<TEntity> entities, long relationId, long targetId, PlayerData.PlayerRelation.RelationType? type = null) where TEntity : PlayerData.PlayerRelation
        {
            return entities.Any(entity =>
            {
                if (entity.RelationId != relationId || entity.TargetId != targetId) return false;
                if (type != null && entity.Type != type) return false;
                return true;
            });
        }

        public static bool ContainsId<TEntity>(this IEnumerable<TEntity> entities, long targetId, PlayerData.PlayerRelation.RelationType? type = null) where TEntity : PlayerData.PlayerRelation
        {
            return entities.Any(entity =>
            {
                if (entity.TargetId != targetId) return false;
                if (type != null && entity.Type != type) return false;
                return true;
            });
        }

        public static void RemoveType<TEntity>(this IList<TEntity> entities, long targetId, PlayerData.PlayerRelation.RelationType type) where TEntity : PlayerData.PlayerRelation
        {
            foreach (TEntity entity in entities.ToArray().Where(entity => entity.TargetId == targetId && entity.Type == type))
            {
                entities.Remove(entity);
            }
        }

        public static bool ContainsType<TEntity>(this IEnumerable<TEntity> entities, PlayerData.PlayerRelation.RelationType type) where TEntity : PlayerData.PlayerRelation
        {
            return entities.Any(entity => entity.Type == type);
        }
    }

    public class PlayerData : ICloneable<PlayerData>
    {
        public void InitDefault()
        {
            DiscordId = null;
            IsDiscordVerified = false;
            Username = "Tanker";
            PasswordHash = Array.Empty<byte>();
            RememberMe = true;
            CountryCode = "EN";
            IsAdmin = true;
            IsBeta = true;
            IsModerator = true;

            CheatSusActions = 0;

            Crystals = 1000000;
            XCrystals = 1000000;
            Experience = 0;
            GoldBonus = 5;
            PremiumExpirationDate = DateTime.MinValue;

            CurrentPresetIndex = 0;
            Presets = new List<PlayerPreset>
            {
                PlayerPreset.Create(this, 0, "Default preset 1"),
                PlayerPreset.Create(this, 1, "Default preset 2"),
                PlayerPreset.Create(this, 2, "Default preset 3")
            };

            FractionIndex = null;
            FractionUserScore = 0;

            DailyBonusCycle = 0;
            DailyBonusNextReceiveDate = DateTime.UtcNow;
            DailyBonusReceivedRewards = new List<DailyBonusReward>();
            DailyBonusZone = 0;

            RegistrationDate = DateTimeOffset.UtcNow;
            LastRecruitReward = null;
            RecruitRewardDay = 0;

            LastSeasonBattles = 0;
            LastSeasonLeagueIndex = 0;
            LastSeasonPlace = 1;
            LastSeasonLeaguePlace = 1;
            SeasonsReputation = new List<PlayerSeasonReputation>
            {
                PlayerSeasonReputation.Create(this, Server.Instance.ServerData.SeasonNumber, 100)
            };

            LeagueChestScore = 0;
            LeagueIndex = 0;
            Reputation = 100;

            Punishments = new List<Punishment>();
            CompletedTutorials = new List<PlayerCompletedTutorial>();

            Dictionary<Entity, List<Entity>> defaultHulls = new Dictionary<Entity, List<Entity>>();
            foreach (Entity hull in GlobalEntities.Hulls.GlobalItems.GetAllItems())
            {
                defaultHulls.Add(hull, HullSkins.GlobalItems.GetAllItems().Where(item => item.GetComponent<ParentGroupComponent>().Key == hull.EntityId).ToList());
            }

            Dictionary<Entity, (List<Entity> Skins, List<Entity> Shells)> defaultWeapons = new Dictionary<Entity, (List<Entity> Skins, List<Entity> Shells)>();
            foreach (Entity weapon in GlobalEntities.Weapons.GlobalItems.GetAllItems())
            {
                List<Entity> skins = WeaponSkins.GlobalItems.GetAllItems().Where(item => item.GetComponent<ParentGroupComponent>().Key == weapon.EntityId).ToList();
                List<Entity> shells = Shells.GlobalItems.GetAllItems().Where(item => item.GetComponent<ParentGroupComponent>().Key == weapon.EntityId).ToList();

                defaultWeapons.Add(weapon, (skins, shells));
            }

            Avatar = 6224;
            Avatars = new List<PlayerAvatar> { PlayerAvatar.Create(this, 6224) };
            Covers = new List<PlayerCover> { PlayerCover.Create(this, -172249613) };
            Graffiti = new List<PlayerGraffiti> { PlayerGraffiti.Create(this, 1001404575) };
            Hulls = new List<PlayerHull>();
            foreach ((Entity hull, List<Entity> skins) in defaultHulls)
            {
                Hulls.Add(PlayerHull.Create(
                    this,
                    hull.EntityId,
                    false,
                    skins: skins.Select(skin => PlayerHullSkin.Create(this, hull.EntityId, skin.EntityId, GlobalEntities.Hulls.DefaultSkins.ContainsValue(skin)))
                ));
            }

            foreach (PlayerHull hull in Hulls)
            {
                hull.SkinId = hull.Skins.First().EntityId;
            }

            Modules = new List<PlayerModule>();
            Paints = new List<PlayerPaint> { PlayerPaint.Create(this, -20020438) };
            Shards = new List<PlayerContainerShards>();
            Weapons = new List<PlayerWeapon>();
            foreach ((Entity weapon, (List<Entity> skins, List<Entity> shells)) in defaultWeapons)
            {
                Weapons.Add(PlayerWeapon.Create(
                    this,
                    weapon.EntityId,
                    false,
                    skins: skins.Select(skin => PlayerWeaponSkin.Create(this, weapon.EntityId, skin.EntityId, GlobalEntities.Weapons.DefaultSkins.ContainsValue(skin))),
                    shellSkins: shells.Select(shell => PlayerWeaponShellSkin.Create(this, weapon.EntityId, shell.EntityId, Shells.DefaultShells.ContainsValue(shell)))
                ));

                PlayerWeapon playerWeapon = Weapons.Single(playerWeapon => playerWeapon.EntityId == weapon.EntityId);

                playerWeapon.SkinId = GlobalEntities.Weapons.DefaultSkins[weapon].EntityId;
                playerWeapon.ShellSkinId = Shells.DefaultShells[weapon].EntityId;
            }

            Hulls.GetById(GlobalEntities.Hulls.GlobalItems.Hunter.EntityId).IsOwned = true;
            Weapons.GetById(GlobalEntities.Weapons.GlobalItems.Smoky.EntityId).IsOwned = true;

            Statistics = PlayerStatistics.Create(this);

            ReceivedFractionsCompetitionReward = false;
            ReceivedReleaseReward = false;
            ShowedFractionsCompetition = false;
            RewardedLeagues = new List<PlayerRewardedLeague>();
        }

        public PlayerData()
        {
        }

        public PlayerData(long uniqueId) : this()
        {
            UniqueId = uniqueId;
        }

        [NotMapped] public Player Player { get; set; }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;

                if (Player?.User is null) return;
                Player?.User.ChangeComponent(new UserUidComponent(value));
            }
        }
        [Key] public long UniqueId { get; set; }

        public long? DiscordId
        {
            get => _discordId;
            set
            {
                _discordId = value;

                if (Player?.User is null) return;
                if (IsDiscordVerified)
                    Player.User.AddOrChangeComponent(new ConfirmedUserEmailComponent(DiscordTag, false));
                else
                    Player.User.AddOrChangeComponent(new UnconfirmedUserEmailComponent(DiscordTag));
            }
        }

        public string DiscordTag => $"{DiscordId}@discord.account";

        public bool IsDiscordVerified
        {
            get => _isDiscordVerified;
            set
            {
                bool wasVerified = _isDiscordVerified;

                _isDiscordVerified = value;

                if (Player?.User is null) return;
                switch (value)
                {
                    case true when !wasVerified:
                        Player.User.RemoveComponent<UnconfirmedUserEmailComponent>();
                        Player.User.AddOrChangeComponent(new ConfirmedUserEmailComponent(DiscordTag, false));
                        break;
                    case false when wasVerified:
                        Player.User.RemoveComponent<ConfirmedUserEmailComponent>();
                        Player.User.AddOrChangeComponent(new UnconfirmedUserEmailComponent(DiscordTag));
                        break;
                }
            }
        }

        [Obsolete("Email replaced with Discord account linking")]
        public string Email
        {
            get => _email;
            set
            {
                _email = value;

                if (Player?.User is null) return;
                Player.User.AddOrChangeComponent(new ConfirmedUserEmailComponent(value, false));
            }
        }

        [Obsolete("Email replaced with Discord account linking")]
        public bool EmailVerified { get; set; }

        [Obsolete("Email replaced with Discord account linking")]
        public bool EmailSubscribed
        {
            get => _emailSubscribed;
            set
            {
                _emailSubscribed = value;

                if (Player?.User is null) return;
                Player.User.AddOrChangeComponent(new ConfirmedUserEmailComponent(Email, value));
            }
        }

        public byte[] PasswordHash { get; set; }
        public string HardwareId { get; set; }
        public byte[] AutoLoginToken { get; set; }

        public int InviteId { get; set; }
        [ForeignKey("InviteId")]
        public Invite Invite { get; set; }

        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                _rememberMe = value;
                if (!value) AutoLoginToken = null;
            }
        }

        public int CheatSusActions { get; set; }

        public string CountryCode
        {
            get => _countryCode;
            set
            {
                _countryCode = value;

                if (Player?.User is null) return;
                Player.User.ChangeComponent(new UserCountryComponent(value));
            }
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                bool wasAdmin = _isAdmin;
                _isAdmin = value;

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
        public bool IsBeta
        {
            get => _isBeta;
            set
            {
                bool wasBeta = _isBeta;
                _isBeta = value;

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
        public bool IsModerator { get; set; }

        public long Crystals
        {
            get => _crystals;
            set
            {
                _crystals = value;

                if (Player?.User is null) return;
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
                Player.User.ChangeComponent(new UserXCrystalsComponent(value));
            }
        }

        public int CurrentBattleSeries { get; set; }
        public long Experience
        {
            get => _experience;
            set
            {
                _experience = value;

                if (Player?.User is null) return;
                Player.User.ChangeComponent(new UserExperienceComponent(value));
            }
        }

        public int GoldBonus
        {
            get => _goldBonus;
            set
            {
                _goldBonus = value;

                if (Player?.User is null) return;
                Player.EntityList.SingleOrDefault(e => e.TemplateAccessor.Template is GoldBonusUserItemTemplate)
                    ?.ChangeComponent<UserItemCounterComponent>(component => component.Count = value);
            }
        }

        public Entity Fraction => FractionIndex != null ? Fractions.ByIndex[FractionIndex.Value] : null;
        public int? FractionIndex { get; set; }

        public long FractionUserScore
        {
            get => _fractionUserScore;
            set
            {
                _fractionUserScore = value;

                if (Player?.User is null) return;
                Player.User.ChangeComponent<FractionUserScoreComponent>(component => component.TotalEarnedPoints = value);
            }
        }

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
        public List<DailyBonusReward> DailyBonusReceivedRewards { get; set; } = new List<DailyBonusReward>();
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

        public Entity League => Leagues.ByIndex[LeagueIndex];
        public int LeagueIndex { get; protected set; }
        public int Reputation
        {
            get => _reputation;
            set
            {
                _reputation = value;

                if (!SeasonsReputation.TryGetBySeason(Server.Instance.ServerData.SeasonNumber, out var reputation))
                {
                    reputation = PlayerSeasonReputation.Create(this, Server.Instance.ServerData.SeasonNumber);
                    SeasonsReputation.Add(reputation);
                }
                reputation.Reputation = value;

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
                _leagueChestScore = value;

                if (Player?.User is null) return;
                Player.User.ChangeComponent<GameplayChestScoreComponent>(component => component.Current = value);
            }
        }

        public long LastSeasonBattles { get; set; }
        public Entity LastSeasonLeague => Leagues.ByIndex[LastSeasonLeagueIndex];
        public int LastSeasonLeagueIndex { get; set; }
        public int LastSeasonPlace { get; set; }
        public int LastSeasonLeaguePlace { get; set; }

        public virtual List<PlayerSeasonReputation> SeasonsReputation { get; set; } = new List<PlayerSeasonReputation>();

        public DateTime PremiumExpirationDate
        {
            get => _premiumExpirationDate;
            set
            {
                _premiumExpirationDate = value;

                if (Player?.User is null) return;
                Player.User.AddOrChangeComponent(new PremiumAccountBoostComponent(PremiumExpirationDate));
            }
        }

        public int CurrentPresetIndex { get; set; }
        public virtual List<PlayerPreset> Presets { get; private set; } = new List<PlayerPreset>();

        public class PlayerRelation
        {
            public static PlayerRelation Create(PlayerData player, PlayerData target, RelationType type)
            {
                return new PlayerRelation()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    Target = target,
                    TargetId = target.UniqueId,
                    Type = type
                };
            }

            public enum RelationType
            {
                Unknown = 0,

                Friend,
                IncomingFriend,
                OutgoingFriend,

                Blocked,

                Reported
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public long RelationId { get; set; }

            public long TargetId { get; set; }
            public virtual PlayerData Target { get; set; }

            public RelationType Type { get; set; }
        }

        public class PlayerRewardedLeague : IEntity
        {
            public static PlayerRewardedLeague Create(PlayerData player, long entityId)
            {
                return new PlayerRewardedLeague()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    EntityId = entityId
                };
            }
            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long EntityId { get; set; }
        }

        public class PlayerSeasonReputation
        {
            public static PlayerSeasonReputation Create(PlayerData player, int season, int reputation = 0)
            {
                return new PlayerSeasonReputation()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    Season = season,
                    Reputation = reputation
                };
            }
            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public int Season { get; set; }
            public int Reputation { get; set; }
        }

        public virtual List<PlayerRelation> Relations { get; private set; } = new List<PlayerRelation>();

        public virtual List<PlayerCompletedTutorial> CompletedTutorials { get; private set; } = new List<PlayerCompletedTutorial>();

        public virtual List<Punishment> Punishments { get; private set; } = new List<Punishment>();

        public class PlayerAvatar : IEntity
        {
            public static PlayerAvatar Create(PlayerData player, long entityId)
            {
                return new PlayerAvatar
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    EntityId = entityId
                };
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long EntityId { get; set; }
        }

        public virtual List<PlayerContainer> Containers { get; set; } = new List<PlayerContainer>();

        public long Avatar
        {
            get => _avatar;
            set
            {
                _avatar = value;

                if (Player?.User is null) return;
                string configPath = Player.GetEntityById(value).TemplateAccessor.ConfigPath;
                string avatarId = Config.GetComponent<AvatarItemComponent>(configPath).Id;

                Player.User.ChangeComponent<UserAvatarComponent>(component => component.Id = avatarId);
            }
        }

        public interface IEntity
        {
            long PlayerId { get; set; }
            long EntityId { get; set; }
        }

        public class PlayerContainer : IEntity
        {
            public static PlayerContainer Create(PlayerData player, long entityId, int count = 0)
            {
                return new PlayerContainer()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    EntityId = entityId,
                    Count = count
                };
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long EntityId { get; set; }

            public int Count { get; set; }
        }

        public class PlayerPaint : IEntity
        {
            public static PlayerPaint Create(PlayerData player, long entityId)
            {
                return new PlayerPaint()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    EntityId = entityId
                };
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long EntityId { get; set; }
        }

        public class PlayerCover : IEntity
        {
            public static PlayerCover Create(PlayerData player, long entityId)
            {
                return new PlayerCover()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    EntityId = entityId
                };
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long EntityId { get; set; }
        }

        public class PlayerGraffiti : IEntity
        {
            public static PlayerGraffiti Create(PlayerData player, long entityId)
            {
                return new PlayerGraffiti()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    EntityId = entityId
                };
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long EntityId { get; set; }
        }

        public interface IPlayerEquipment
        {
            bool IsOwned { get; set; }
        }

        public class PlayerHull : IEntity, IPlayerEquipment
        {
            public static PlayerHull Create(PlayerData player, long entityId, bool isOwned, long xp = 0, IEnumerable<PlayerHullSkin> skins = null)
            {
                var hull = new PlayerHull()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    EntityId = entityId,
                    IsOwned = isOwned,
                    Xp = xp
                };
                if (skins != null) hull.Skins.AddRange(skins);

                return hull;
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long EntityId { get; set; }

            public bool IsOwned { get; set; }
            public long Xp { get; set; }

            public long SkinId { get; set; }

            public virtual List<PlayerHullSkin> Skins { get; set; } = new List<PlayerHullSkin>();
        }

        public class PlayerWeapon : IEntity, IPlayerEquipment
        {
            public static PlayerWeapon Create(PlayerData player, long entityId, bool isOwned, long xp = 0, IEnumerable<PlayerWeaponSkin> skins = null, IEnumerable<PlayerWeaponShellSkin> shellSkins = null)
            {
                var weapon = new PlayerWeapon()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    EntityId = entityId,
                    IsOwned = isOwned,
                    Xp = xp
                };
                if (skins != null) weapon.Skins.AddRange(skins);
                if (shellSkins != null) weapon.ShellSkins.AddRange(shellSkins);

                return weapon;
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long EntityId { get; set; }

            public bool IsOwned { get; set; }
            public long Xp { get; set; }

            public long SkinId { get; set; }
            public long ShellSkinId { get; set; }

            public virtual List<PlayerWeaponSkin> Skins { get; set; } = new List<PlayerWeaponSkin>();
            public virtual List<PlayerWeaponShellSkin> ShellSkins { get; set; } = new List<PlayerWeaponShellSkin>();
        }

        public class PlayerHullSkin : IEntity, IPlayerEquipment
        {
            public static PlayerHullSkin Create(PlayerData player, long hullId, long entityId, bool isOwned)
            {
                return new PlayerHullSkin()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    HullId = hullId,
                    EntityId = entityId,
                    IsOwned = isOwned
                };
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long HullId { get; set; }

            // [ForeignKey("PlayerId, EntityId")]
            public virtual PlayerHull Hull { get; set; }

            public long EntityId { get; set; }

            public bool IsOwned { get; set; }
        }

        public class PlayerWeaponSkin : IEntity, IPlayerEquipment
        {
            public static PlayerWeaponSkin Create(PlayerData player, long weaponId, long entityId, bool isOwned)
            {
                return new PlayerWeaponSkin()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    WeaponId = weaponId,
                    EntityId = entityId,
                    IsOwned = isOwned
                };
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long WeaponId { get; set; }

            // [ForeignKey("PlayerId, EntityId")]
            public virtual PlayerWeapon Weapon { get; set; }

            public long EntityId { get; set; }

            public bool IsOwned { get; set; }
        }

        public class PlayerWeaponShellSkin : IEntity, IPlayerEquipment
        {
            public static PlayerWeaponShellSkin Create(PlayerData player, long weaponId, long entityId, bool isOwned)
            {
                return new PlayerWeaponShellSkin()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    WeaponId = weaponId,
                    EntityId = entityId,
                    IsOwned = isOwned
                };
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long WeaponId { get; set; }

            // [ForeignKey("PlayerId, EntityId")]
            public virtual PlayerWeapon Weapon { get; set; }

            public long EntityId { get; set; }

            public bool IsOwned { get; set; }
        }

        public class PlayerModule : IEntity
        {
            public static PlayerModule Create(PlayerData player, long entityId, int level = 0, int cards = 0)
            {
                return new PlayerModule()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    EntityId = entityId,
                    Level = level,
                    Cards = cards
                };
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long EntityId { get; set; }

            public int Level { get; set; }
            public int Cards { get; set; }
        }

        public class PlayerContainerShards : IEntity
        {
            public static PlayerContainerShards Create(PlayerData player, long entityId, int amount = 0)
            {
                return new PlayerContainerShards()
                {
                    Player = player,
                    PlayerId = player.UniqueId,
                    EntityId = entityId,
                    Amount = amount
                };
            }

            public long PlayerId { get; set; }
            [ForeignKey("PlayerId")]
            public virtual PlayerData Player { get; set; }

            public long EntityId { get; set; }

            public int Amount { get; set; }
        }

        public virtual List<PlayerPaint> Paints { get; set; } = new List<PlayerPaint>();
        public virtual List<PlayerCover> Covers { get; set; } = new List<PlayerCover>();
        public virtual List<PlayerGraffiti> Graffiti { get; set; } = new List<PlayerGraffiti>();
        public virtual List<PlayerAvatar> Avatars { get; set; } = new List<PlayerAvatar>();

        public virtual List<PlayerHull> Hulls { get; set; } = new List<PlayerHull>();
        public virtual List<PlayerWeapon> Weapons { get; set; } = new List<PlayerWeapon>();

        public virtual List<PlayerModule> Modules { get; set; } = new List<PlayerModule>();

        public virtual List<PlayerContainerShards> Shards { get; set; } = new List<PlayerContainerShards>();

        public virtual PlayerStatistics Statistics { get; set; }

        public bool ReceivedFractionsCompetitionReward { get; set; }
        public bool ReceivedReleaseReward { get; set; }
        public bool ReceivedLastSeasonReward { get; set; }
        public bool ShowedFractionsCompetition { get; set; }
        public virtual List<PlayerRewardedLeague> RewardedLeagues { get; set; } = new List<PlayerRewardedLeague>();

        public bool OwnsMarketItem(Entity marketItem) => Avatars.ToIds()
            .Concat(Covers.ToIds())
            .Concat(Graffiti.ToIds())
            .Concat(Hulls.ToOwnedIds())
            .Concat(Hulls.SelectMany(hull => hull.Skins).ToOwnedIds())
            .Concat(Paints.ToIds())
            .Concat(Weapons.ToOwnedIds())
            .Concat(Weapons.SelectMany(weapon => weapon.Skins).ToOwnedIds())
            .Concat(Weapons.SelectMany(weapon => weapon.ShellSkins).ToOwnedIds())
            .Contains(marketItem.EntityId);

        public bool IsPremium => PremiumExpirationDate > DateTime.UtcNow;

        private void SetLeague(int reputation)
        {
            LeagueIndex = reputation switch
            {
                <= 139 => 0,
                >= 140 and <= 999 => 1,
                >= 1000 and <= 2999 => 2,
                >= 3000 and <= 4499 => 3,
                >= 4500 => 4
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
            DailyBonusReceivedRewards.Add(DailyBonusReward.Create(this, code));
            Player.User.ChangeComponent<UserDailyBonusReceivedRewardsComponent>();
        }
        public void ResetDailyBonusRewards()
        {
            DailyBonusReceivedRewards.Clear();
            Player.User.ChangeComponent<UserDailyBonusReceivedRewardsComponent>();
        }

        public void AddIncomingFriend(PlayerData user)
        {
            Relations.Add(PlayerRelation.Create(this, user, PlayerRelation.RelationType.IncomingFriend));
        }

        public void AddOutgoingFriend(PlayerData user)
        {
            Relations.Add(PlayerRelation.Create(this, user, PlayerRelation.RelationType.OutgoingFriend));
        }

        public void AddAcceptedFriend(PlayerData user)
        {
            Relations.RemoveType(user.UniqueId, PlayerRelation.RelationType.IncomingFriend);
            Relations.RemoveType(user.UniqueId, PlayerRelation.RelationType.OutgoingFriend);
            Relations.Add(PlayerRelation.Create(this, user, PlayerRelation.RelationType.Friend));
        }

        public void RemoveFriend(PlayerData user)
        {
            Relations.RemoveType(user.UniqueId, PlayerRelation.RelationType.IncomingFriend);
            Relations.RemoveType(user.UniqueId, PlayerRelation.RelationType.OutgoingFriend);
            Relations.RemoveType(user.UniqueId, PlayerRelation.RelationType.Friend);
        }

        public void ChangeBlockedPlayer(PlayerData user)
        {
            if (Relations.ContainsId(user.UniqueId, PlayerRelation.RelationType.Blocked))
                Relations.RemoveType(user.UniqueId, PlayerRelation.RelationType.Blocked);
            else
                Relations.Add(PlayerRelation.Create(this, user, PlayerRelation.RelationType.Blocked));
        }

        public void AddReportedPlayer(PlayerData user)
        {
            Relations.Add(PlayerRelation.Create(this, user, PlayerRelation.RelationType.Reported));
        }

        public void AddCompletedTutorial(long tutorialId)
        {
            CompletedTutorials.Add(PlayerCompletedTutorial.Create(this, tutorialId));

            if (Player?.User is null) return;
            Player.User.ChangeComponent<TutorialCompleteIdsComponent>();
        }

        public void AddHullXp(int xp, Entity hull)
        {
            Hulls.GetById(hull.EntityId).Xp += xp;

            ResourceManager
                .GetUserItem(Player, hull)
                .ChangeComponent<ExperienceItemComponent>(component => component.Experience = Hulls.GetById(hull.EntityId).Xp);
        }
        public void AddWeaponXp(int xp, Entity weapon)
        {
            Weapons.GetById(weapon.EntityId).Xp += xp;

            ResourceManager
                .GetUserItem(Player, weapon)
                .ChangeComponent<ExperienceItemComponent>(component => component.Experience = Weapons.GetById(weapon.EntityId).Xp);
        }

        public void UpgradeModule(Entity marketItem, bool assemble = false)
        {
            if (!Modules.ContainsId(marketItem.GetComponent<ParentGroupComponent>().Key)) return;

            long id = marketItem.GetComponent<ParentGroupComponent>().Key;
            PlayerModule info = Modules.GetById(id);

            Entity moduleUserItem = ResourceManager.GetModuleUserItem(Player, id);
            Entity cardUserItem = Player.EntityList.Single(e => e.TemplateAccessor.Template is ModuleCardUserItemTemplate && e.GetComponent<ParentGroupComponent>().Key == id);

            if (!assemble) info.Level++;
            info.Cards -= moduleUserItem.GetComponent<ModuleCardsCompositionComponent>().AllPrices[info.Level].Cards;

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
        private long? _discordId;
        private bool _isDiscordVerified;
        private bool _emailSubscribed;
        private string _email;
        private bool _isAdmin;
        private bool _isBeta;
        private long _experience;
        private bool _rememberMe;
        private DateTime _premiumExpirationDate;
    }
}
