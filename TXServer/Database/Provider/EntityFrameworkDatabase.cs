#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Serilog;
using TXServer.Core;
using TXServer.Core.ChatCommands;
using TXServer.Core.Data.Database;
using TXServer.Core.Logging;
using TXServer.Database.Entity;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Database.Provider
{
    public abstract class EntityFrameworkDatabase : DbContext, IDatabase
    {
        private static readonly ILogger Logger = Log.Logger.ForType<EntityFrameworkDatabase>();

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            // builder.UseLoggerFactory(LoggerFactory.Create(builder => builder
            //     .AddFilter((category, level) => true)
            //     .AddConsole()
            // ));
            builder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var converter = new ValueConverter<ECSSystem.Base.Entity, long?>(
                entity => entity != null ? entity.EntityId : null,
                entityId => ResourceManager.GetGlobalEntities().SingleOrDefault(entity => entity.EntityId == entityId)
            );

            builder.Entity<PlayerPreset>(entity =>
            {
                entity.Property(preset => preset.Hull).HasConversion(converter);
                entity.Property(preset => preset.HullPaint).HasConversion(converter);
                entity.Property(preset => preset.HullSkin).HasConversion(converter);

                entity.Property(preset => preset.Weapon).HasConversion(converter);
                entity.Property(preset => preset.WeaponPaint).HasConversion(converter);
                entity.Property(preset => preset.WeaponSkin).HasConversion(converter);
                entity.Property(preset => preset.WeaponShellSkin).HasConversion(converter);

                entity.Property(preset => preset.Graffiti).HasConversion(converter);
            });

            // foreach (IMutableProperty property in builder.Model.GetEntityTypes().SelectMany(type => type.GetProperties()))
            // {
            //     if (property.ClrType == typeof(ECSSystem.Base.Entity))
            //         property.SetValueConverter(converter);
            // }

            foreach (IMutableProperty property in builder.Model.GetEntityTypes().SelectMany(type => type.GetProperties()))
            {
                if (property.ClrType == typeof(TimeSpan) || property.ClrType == typeof(TimeSpan?))
                    property.SetValueConverter(new TimeSpanToTicksConverter());
            }

            builder.Entity<PlayerData>(entity =>
            {
            });

            builder.Entity<PlayerData.PlayerHull>(entity =>
            {
                entity.HasMany(player => player.Skins)
                    .WithOne(skin => skin.Hull)
                    .HasForeignKey(skin => new { skin.PlayerId, skin.EntityId });
            });

            builder.Entity<PlayerData.PlayerWeapon>(entity =>
            {
                entity.HasMany(weapon => weapon.Skins)
                    .WithOne(skin => skin.Weapon)
                    .HasForeignKey(skin => new { skin.PlayerId, skin.EntityId });

                entity.HasMany(weapon => weapon.ShellSkins)
                    .WithOne(skin => skin.Weapon)
                    .HasForeignKey(skin => new { skin.PlayerId, skin.EntityId });
            });

            builder.Entity<PlayerData.PlayerRelation>(entity =>
            {
                entity.HasKey(relation => new { relation.PlayerId, relation.RelationId });

                entity.HasOne(relation => relation.Player)
                    .WithMany(player => player.Relations)
                    .HasForeignKey(relation => relation.PlayerId);
            });

            builder.Entity<PlayerData.PlayerRewardedLeague>(entity =>
            {
                entity.HasKey(league => new { league.PlayerId, league.EntityId });
            });

            builder.Entity<PlayerData.PlayerSeasonReputation>(entity =>
            {
                entity.HasKey(reputation => new { reputation.PlayerId, reputation.Season });
            });

            builder.Entity<Punishment>(entity =>
            {
                entity.HasKey(punishment => new { punishment.PlayerId, punishment.PunishmentId });
            });

            builder.Entity<PlayerData.PlayerPaint>(entity =>
            {
                entity.HasKey(paint => new { paint.PlayerId, paint.EntityId });
            });

            builder.Entity<PlayerData.PlayerCover>(entity =>
            {
                entity.HasKey(paint => new { paint.PlayerId, paint.EntityId });
            });

            builder.Entity<PlayerData.PlayerGraffiti>(entity =>
            {
                entity.HasKey(graffiti => new { graffiti.PlayerId, graffiti.EntityId });
            });

            builder.Entity<PlayerData.PlayerAvatar>(entity =>
            {
                entity.HasKey(avatar => new { avatar.PlayerId, avatar.EntityId });
            });

            builder.Entity<PlayerData.PlayerHull>(entity =>
            {
                entity.HasKey(hull => new { hull.PlayerId, hull.EntityId });
            });

            builder.Entity<PlayerData.PlayerWeapon>(entity =>
            {
                entity.HasKey(weapon => new { weapon.PlayerId, weapon.EntityId });
            });

            builder.Entity<PlayerData.PlayerHullSkin>(entity =>
            {
                entity.HasKey(skin => new { skin.PlayerId, skin.HullId, skin.EntityId });
                entity.HasOne(skin => skin.Hull)
                    .WithMany(hull => hull.Skins)
                    .HasForeignKey(skin => new { skin.PlayerId, skin.HullId });
            });

            builder.Entity<PlayerData.PlayerWeaponSkin>(entity =>
            {
                entity.HasKey(skin => new { skin.PlayerId, skin.WeaponId, skin.EntityId });
                entity.HasOne(skin => skin.Weapon)
                    .WithMany(weapon => weapon.Skins)
                    .HasForeignKey(skin => new { skin.PlayerId, skin.WeaponId });
            });

            builder.Entity<PlayerData.PlayerWeaponShellSkin>(entity =>
            {
                entity.HasKey(skin => new { skin.PlayerId, skin.WeaponId, skin.EntityId });
                entity.HasOne(skin => skin.Weapon)
                    .WithMany(weapon => weapon.ShellSkins)
                    .HasForeignKey(skin => new { skin.PlayerId, skin.WeaponId });
            });

            builder.Entity<PlayerData.PlayerModule>(entity =>
            {
                entity.HasKey(module => new { module.PlayerId, module.EntityId });
            });

            builder.Entity<PlayerPreset>(entity =>
            {
                entity.HasKey(preset => new { preset.PlayerId, preset.Index });
            });

            builder.Entity<PlayerData.PlayerContainerShards>(entity =>
            {
                entity.HasKey(shards => new { shards.PlayerId, shards.EntityId });
            });

            builder.Entity<PlayerData.PlayerContainer>(entity =>
            {
                entity.HasKey(container => new { container.PlayerId, container.EntityId });
            });

            builder.Entity<DailyBonusReward>(entity =>
            {
                entity.HasKey(reward => new { reward.PlayerId, reward.EntityId });
            });

            builder.Entity<PlayerCompletedTutorial>(entity =>
            {
                entity.HasKey(tutorial => new { tutorial.PlayerId, tutorial.EntityId });
            });

            builder.Entity<PlayerPresetModule>(entity =>
            {
                entity.HasKey(module => new { module.PlayerId, module.PresetIndex, module.Slot });
            });

            builder.Entity<PlayerStatistics>(entity =>
            {
                entity.HasKey(statistics => new { statistics.PlayerId });
            });

            builder.Entity<PlayerData>(entity =>
            {
                entity.Ignore(player => player.Player);
            });

            builder.Entity<PlayerPreset>(entity =>
            {
                entity.Ignore(player => player.Entity);
            });
        }

        public DbSet<PlayerData> Players { get; protected set; } = null!;
        public DbSet<PlayerStatistics> PlayerStatistics { get; protected set; } = null!;

        public DbSet<PlayerData.PlayerRelation> Relations { get; protected set; } = null!;
        public DbSet<Punishment> Punishments { get; protected set; } = null!;

        public DbSet<PlayerData.PlayerRewardedLeague> RewardedLeagues { get; protected set; } = null!;
        public DbSet<PlayerData.PlayerSeasonReputation> SeasonReputations { get; protected set; } = null!;

        public DbSet<PlayerData.PlayerPaint> Paints { get; protected set; } = null!;
        public DbSet<PlayerData.PlayerCover> Covers { get; protected set; } = null!;
        public DbSet<PlayerData.PlayerGraffiti> Graffities { get; protected set; } = null!;
        public DbSet<PlayerData.PlayerAvatar> Avatars { get; protected set; } = null!;

        public DbSet<PlayerData.PlayerHull> Hulls { get; protected set; } = null!;
        public DbSet<PlayerData.PlayerHullSkin> HullSkins { get; protected set; } = null!;

        public DbSet<PlayerData.PlayerWeapon> Weapons { get; protected set; } = null!;
        public DbSet<PlayerData.PlayerWeaponSkin> WeaponSkins { get; protected set; } = null!;
        public DbSet<PlayerData.PlayerWeaponShellSkin> WeaponShellSkins { get; protected set; } = null!;

        public DbSet<PlayerData.PlayerModule> Modules { get; protected set; } = null!;

        public DbSet<PlayerPreset> Presets { get; protected set; } = null!;
        public DbSet<PlayerPresetModule> PresetModules { get; protected set; } = null!;

        public DbSet<PlayerData.PlayerContainer> Containers { get; protected set; } = null!;
        public DbSet<PlayerData.PlayerContainerShards> ContainerShards { get; protected set; } = null!;

        public DbSet<DailyBonusReward> DailyBonusRewards { get; protected set; } = null!;
        public DbSet<PlayerCompletedTutorial> CompletedTutorials { get; protected set; } = null!;

        public DbSet<ServerData> Servers { get; protected set; } = null!;

        public DbSet<Invite> Invites { get; protected set; } = null!;
        public DbSet<BlockedUsername> BlockedUsernames { get; protected set; } = null!;

        // PlayerData

        public virtual PlayerData? GetPlayerData(string username)
        {
            lock (this)
            {
                return Players.IncludePlayer().SingleOrDefault(player => player.Username == username);
            }
        }

        public virtual PlayerData? GetPlayerDataById(long id)
        {
            lock (this)
            {
                return Players.IncludePlayer().SingleOrDefault(player => player.UniqueId == id);
            }
        }

        [Obsolete("Email replaced with Discord account linking")]
        public virtual PlayerData? GetPlayerDataByEmail(string email)
        {
            lock (this)
            {
                return Players.IncludePlayer().SingleOrDefault(player => player.Email == email);
            }
        }

        public bool IsUsernameAvailable(string username)
        {
            lock (this)
            {
                return !Players.Any(player => player.Username == username);
            }
        }

        [Obsolete("Email replaced with Discord account linking")]
        public bool IsEmailAvailable(string email)
        {
            lock (this)
            {
                return !Players.Any(player => player.Email == email);
            }
        }

        public bool TryGetInvite(string code, [MaybeNullWhen(false)] out Invite invite)
        {
            lock (this)
            {
                invite = Invites.SingleOrDefault(invite => invite.Code == code);
                return invite != null;
            }
        }

        public void Save()
        {
            lock (this)
            {
                ChangeTracker.DetectChanges();
                foreach (EntityEntry entry in ChangeTracker.Entries())
                {
                    var entity = entry.Entity;

                    if (entry.State == EntityState.Added)
                    {
                        Logger.Verbose("Created {Type}: {Entity}", entry.Entity.GetType().Name, entity);
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        var originalValues = Entry(entity).OriginalValues;
                        var currentValues = Entry(entity).CurrentValues;

                        foreach (IProperty property in originalValues.Properties)
                        {
                            var original = originalValues[property];
                            var current = currentValues[property];

                            if (!Equals(original, current))
                            {
                                Logger.Verbose("Changed {Type}.{Property}: {Original} -> {Current}", entity.GetType().Name, property.Name, original, current);
                            }
                        }
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        Logger.Verbose("Deleted {Type}: {Entity}", entry.Entity.GetType().Name, entity);
                    }
                }

                int changes = SaveChanges();
                if (changes > 0) Logger.Debug("Database changes: {Changes}", changes);
            }
        }

        public bool Shutdown()
        {
            // Let the driver take care of the connections
            return true;
        }
    }
}
