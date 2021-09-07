#nullable enable

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TXServer.Core.ChatCommands;
using TXServer.Database.Entity;

namespace TXServer.Core.Data.Database
{
    public interface IDatabase
    {
        DbSet<PlayerData> Players { get; }
        DbSet<PlayerStatistics> PlayerStatistics { get; }

        DbSet<PlayerData.PlayerRelation> Relations { get; }
        DbSet<Punishment> Punishments { get;}

        DbSet<PlayerData.PlayerRewardedLeague> RewardedLeagues { get; }
        DbSet<PlayerData.PlayerSeasonReputation> SeasonReputations { get; }

        DbSet<PlayerData.PlayerAvatar> Avatars { get; }
        DbSet<PlayerData.PlayerPaint> Paints { get; }
        DbSet<PlayerData.PlayerCover> Covers { get; }
        DbSet<PlayerData.PlayerGraffiti> Graffities { get;}

        DbSet<PlayerData.PlayerHull> Hulls { get;  }
        DbSet<PlayerData.PlayerHullSkin> HullSkins { get; }

        DbSet<PlayerData.PlayerWeapon> Weapons { get;}
        DbSet<PlayerData.PlayerWeaponSkin> WeaponSkins { get; }
        DbSet<PlayerData.PlayerWeaponShellSkin> WeaponShellSkins { get;  }

        DbSet<PlayerData.PlayerModule> Modules { get; }

        DbSet<PlayerPreset> Presets { get; }
        DbSet<PlayerPresetModule> PresetModules { get; }

        DbSet<PlayerData.PlayerContainer> Containers { get; }
        DbSet<PlayerData.PlayerContainerShards> ContainerShards { get;  }

        DbSet<DailyBonusReward> DailyBonusRewards { get; }
        DbSet<PlayerCompletedTutorial> CompletedTutorials { get; }

        DbSet<ServerData> Servers { get; }

        DbSet<Invite> Invites { get; }
        DbSet<BlockedUsername> BlockedUsernames { get; }

        // PlayerData

        PlayerData? GetPlayerData(string username);
        PlayerData? GetPlayerDataById(long id);
        PlayerData? GetPlayerDataByEmail(string email);

        bool IsUsernameAvailable(string username);
        bool IsEmailAvailable(string email);

        bool TryGetInvite(string code, [MaybeNullWhen(false)] out Invite invite);

        void Save();

        // bool Startup();
        bool Shutdown();
    }
}
