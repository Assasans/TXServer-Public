using System.ComponentModel.DataAnnotations.Schema;

namespace TXServer.Core
{
    public class PlayerStatistics
    {
        public static PlayerStatistics Create(PlayerData player)
        {
            return new PlayerStatistics()
            {
                Player = player,
                PlayerId = player.UniqueId
            };
        }

        public long PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        public virtual PlayerData Player { get; set; }

        public long Healed { get; set; }
        public long CtfCarriageScore { get; set; }
        public long Score { get; set; }
        public long EnergyCompensation { get; set; }
        public long BattlesParticipated { get; set; }
        public long Deaths { get; set; }
        public long CtfReturnScore { get; set; }
        public long Suicides { get; set; }
        public long AllCustomBattlesParticipated { get; set; }
        public long HealXp { get; set; }
        public long KillScore { get; set; }
        public long Victories { get; set; }
        public long CtfPlayed { get; set; }
        public long Defeats { get; set; }
        public long DmPlayed { get; set; }
        public long Hits { get; set; }
        public long KillAssistXp { get; set; }
        public long KillXp { get; set; }
        public long CurrentWinningStreak { get; set; }
        public long Energy { get; set; }
        public long CtfCarriageXp { get; set; }
        public long CtfReturnXp { get; set; }
        public long Shots { get; set; }
        public long Draws { get; set; }
        public long AllBattlesParticipated { get; set; }
        public long PunishmentScore { get; set; }
        public long Kills { get; set; }
        public long HealScore { get; set; }
        public long TdmPlayed { get; set; }
        public long Xp { get; set; }
        public long KillAssistScore { get; set; }
        public long BattlesParticipatedInSeason { get; set; }
        public long CustomBattlesParticipated { get; set; }
        public long Golds { get; set; }
    }
}
