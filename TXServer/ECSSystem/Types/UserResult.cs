using System;
using System.Collections.Generic;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Types
{
	public class UserResult
	{
		public UserResult(Player player, List<UserResult> teamUserResults)
		{
			UserId = player.User.EntityId;
			Uid = player.User.GetComponent<UserUidComponent>().Uid;
			Rank = player.User.GetComponent<UserRankComponent>().Rank;
			AvatarId = player.User.GetComponent<UserAvatarComponent>().Id;
			EnterTime = DateTime.Now.Ticks;
			Place = teamUserResults.Count;
			foreach (Entity module in player.CurrentPreset.Modules.Values)
            {
				if (module != null)
					Modules.Add(new ModuleInfo(module.GetComponent<MarketItemGroupComponent>().Key, module.GetComponent<ModuleUpgradeLevelComponent>().Level));
			}
			League = Leagues.GlobalItems.Silver;
		}

		public long UserId { get; set; }
		public string Uid { get; set; }
		public int Rank { get; set; }
		public string AvatarId { get; set; }
		public long BattleUserId { get; set; } = 0;
		public double ReputationInBattle { get; set; } = 0;
		public long EnterTime { get; set; }
		public int Place { get; set; }
		public int Kills { get; set; } = 0;
		public int KillAssists { get; set; } = 0;
		public int KillStrike { get; set; } = 0;
		public int Deaths { get; set; } = 0;
		public int Damage { get; set; } = 0;
		public int Score { get; set; } = 0;
		public int ScoreWithoutPremium { get; set; }
		public int ScoreToExperience { get; set; } = 0;
		public int RankExpDelta { get; set; } = 0;
		public int ItemsExpDelta { get; set; } = 0;
		public int Flags { get; set; }
		public int FlagAssists { get; set; }
		public int FlagReturns { get; set; }
		public long WeaponId { get; set; }
		public long HullId { get; set; }
		public long PaintId { get; set; }
		public long CoatingId { get; set; }
		public long HullSkinId { get; set; }
		public long WeaponSkinId { get; set; }
		public List<ModuleInfo> Modules { get; set; } = new();
		public int BonusesTaken { get; set; }
		public bool UnfairMatching { get; set; }
		public bool Deserted { get; set; }
		public Entity League { get; set; }
	}
}
