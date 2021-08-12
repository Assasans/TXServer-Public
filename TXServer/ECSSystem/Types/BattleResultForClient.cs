using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.Types
{
	public class BattleResultForClient
	{
        public BattleResultForClient(BaseBattlePlayer battlePlayer)
        {
            Battle battle = battlePlayer.Battle;
            BattleId = battle.BattleEntity.EntityId;
            MapId = battle.MapEntity.EntityId;
            BattleMode = battle.Params.BattleMode;
            MatchMakingModeType = BattleType.RATING;
            Custom = !battle.IsMatchMaking;
            Spectator = battlePlayer.GetType() == typeof(Spectator);
            PersonalResult = (battlePlayer as BattleTankPlayer)?.MatchPlayer.PersonalBattleResult;

            if (battle.ModeHandler is Battle.TeamBattleHandler handler)
            {
                RedTeamScore = handler.RedTeamEntity.GetComponent<TeamScoreComponent>().Score;
                BlueTeamScore = handler.BlueTeamEntity.GetComponent<TeamScoreComponent>().Score;

                RedUsers = handler.RedTeamResults.ToList();
                BlueUsers = handler.BlueTeamResults.ToList();
            }
            else
            {
                DmUsers = ((Battle.DMHandler)battle.ModeHandler).Results.ToList();
                DmScore = DmUsers.Sum(user => user.Kills);
            }
        }

        public long BattleId { get; set; }
		public long MapId { get; set; }
		public BattleMode BattleMode { get; set; }
		public BattleType MatchMakingModeType { get; set; }
		public bool Custom { get; set; }
		public int RedTeamScore { get; set; } = 0;
		public int BlueTeamScore { get; set; } = 0;
		public int DmScore { get; set; } = 0;
		public List<UserResult> RedUsers { get; set; } = new();
		public List<UserResult> BlueUsers { get; set; } = new();
		public List<UserResult> DmUsers { get; set; } = new();
		public bool Spectator { get; set; }
		[OptionalMapped]
		public PersonalBattleResultForClient PersonalResult { get; set; }
	}
}
