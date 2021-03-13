using System.Collections.Generic;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.Types
{
	public class BattleResultForClient
	{
        public BattleResultForClient(Battle battle, PersonalBattleResultForClient personalResult)
        {
            BattleId = battle.BattleEntity.EntityId;
			MapId = battle.MapEntity.EntityId;
			BattleMode = battle.BattleParams.BattleMode;
			MatchMakingModeType = BattleType.RATING;
			Custom = !battle.IsMatchMaking;
			RedTeamScore = battle.RedTeamEntity.GetComponent<TeamScoreComponent>().Score;
			BlueTeamScore = battle.BlueTeamEntity.GetComponent<TeamScoreComponent>().Score;
			DmScore = 3;
			RedUsers = battle.RedTeamResults;
			BlueUsers = battle.BlueTeamResults;
			DmUsers = battle.DMTeamResults;
			Spectator = false;
			PersonalResult = personalResult;
        }

        public long BattleId { get; set; }
		public long MapId { get; set; }
		public BattleMode BattleMode { get; set; }
		public BattleType MatchMakingModeType { get; set; }
		public bool Custom { get; set; }
		public int RedTeamScore { get; set; }
		public int BlueTeamScore { get; set; }
		public int DmScore { get; set; }
		public List<UserResult> RedUsers { get; set; }
		public List<UserResult> BlueUsers { get; set; }
		public List<UserResult> DmUsers { get; set; }
		public bool Spectator { get; set; }
		[OptionalMapped]
		public PersonalBattleResultForClient PersonalResult { get; set; }
	}
}
