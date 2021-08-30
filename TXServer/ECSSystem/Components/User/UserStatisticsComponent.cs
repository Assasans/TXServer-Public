using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
	[SerialVersionUID(1499174753575)]
	public class UserStatisticsComponent : Component
    {
        private readonly Player _player;

        public UserStatisticsComponent(Player player)
        {
            _player = player;
        }

		private static readonly Dictionary<string, Func<PlayerStatistics, long>> PropertyMap = new()
		{
			["HEALED"] = statistics => statistics.Healed,
			["CTF_CARRIAGE_SCORE"] = statistics => statistics.CtfCarriageScore,
			["SCORE"] = statistics => statistics.Score,
			["ENERGY_COMPENSATION"] = statistics => statistics.EnergyCompensation,
			["BATTLES_PARTICIPATED"] = statistics => statistics.BattlesParticipated,
			["DEATHS"] = statistics => statistics.Deaths,
			["CTF_RETURN_SCORE"] = statistics => statistics.CtfReturnScore,
			["SUICIDES"] = statistics => statistics.Suicides,
			["ALL_CUSTOM_BATTLES_PARTICIPATED"] = statistics => statistics.AllCustomBattlesParticipated,
			["HEAL_XP"] = statistics => statistics.HealXp,
			["KILL_SCORE"] = statistics => statistics.KillScore,
			["VICTORIES"] = statistics => statistics.Victories,
			["CTF_PLAYED"] = statistics => statistics.CtfPlayed,
			["DEFEATS"] = statistics => statistics.Defeats,
			["DM_PLAYED"] = statistics => statistics.DmPlayed,
			["HITS"] = statistics => statistics.Hits,
			["KILL_ASSIST_XP"] = statistics => statistics.KillAssistXp,
			["KILL_XP"] = statistics => statistics.KillXp,
			["CURRENT_WINNING_STREAK"] = statistics => statistics.CurrentWinningStreak,
			["ENERGY"] = statistics => statistics.Energy,
			["CTF_CARRIAGE_XP"] = statistics => statistics.CtfCarriageXp,
			["CTF_RETURN_XP"] = statistics => statistics.CtfReturnXp,
			["SHOTS"] = statistics => statistics.Shots,
			["DRAWS"] = statistics => statistics.Draws,
			["ALL_BATTLES_PARTICIPATED"] = statistics => statistics.AllBattlesParticipated,
			["PUNISHMENT_SCORE"] = statistics => statistics.PunishmentScore,
			["KILLS"] = statistics => statistics.Kills,
			["HEAL_SCORE"] = statistics => statistics.HealScore,
			["TDM_PLAYED"] = statistics => statistics.TdmPlayed,
			["XP"] = statistics => statistics.Xp,
			["KILL_ASSIST_SCORE"] = statistics => statistics.KillAssistScore,
			["BATTLES_PARTICIPATED_IN_SEASON"] = statistics => statistics.BattlesParticipatedInSeason,
			["CUSTOM_BATTLES_PARTICIPATED"] = statistics => statistics.CustomBattlesParticipated,
			["GOLDS"] = statistics => statistics.Golds
		};

        public ReadOnlyDictionary<string, long> Statistics
        {
            get
            {
                Dictionary<string, long> statistics = new Dictionary<string, long>();
                foreach ((string key, Func<PlayerStatistics, long> mapper) in PropertyMap)
                {
                    statistics.Add(key, mapper(_player.Data.Statistics));
                }

                return new ReadOnlyDictionary<string, long>(statistics);
            }
        }
	}
}
