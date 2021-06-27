using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Types
{
	public class PersonalBattleResultForClient
	{
        public PersonalBattleResultForClient(Player player)
        {
            _player = player;

            _oldRankExp = _player.Data.Experience;
            PrevLeague = _player.Data.League;
            Reward = BattleRewards.GlobalItems.XCrystalBonus;
			UserTeamColor = player.User.GetComponent<TeamColorComponent>().TeamColor;
        }

        public void FinalizeResult()
        {
            TeamBattleResult = _player.BattlePlayer.Battle.ModeHandler.TeamBattleResultFor(_player.BattlePlayer);
            League = _player.Data.League;

            RankExp = (int) _player.Data.Experience;
            RankExpDelta = (int) (RankExp - _oldRankExp);

            // todo: calculate earned reputation
            Reputation = _player.Data.Reputation + (TeamBattleResult == TeamBattleResult.DEFEAT ? -11 : 11);
            ReputationDelta = Reputation - _player.Data.Reputation;
            _player.Data.Reputation = (int) Reputation;
        }

        private readonly Player _player;
        private readonly long _oldRankExp;


        public TeamColor UserTeamColor { get; set; }
		public TeamBattleResult TeamBattleResult { get; set; }
		public int Energy { get; set; } = 0;
		public int EnergyDelta { get; set; } = 0;
		public int CrystalsForExtraEnergy { get; set; } = 0;
		[OptionalMapped]
		public EnergySource MaxEnergySource { get; set; } = EnergySource.MVP_BONUS;
        public int CurrentBattleSeries { get; set; }
		public int MaxBattleSeries { get; set; } = 0;
		public float ScoreBattleSeriesMultiplier { get; set; } = 0;
		public int RankExp { get; set; }
		public int RankExpDelta { get; set; }
		public int WeaponExp { get; set; } = 0;
		public int TankLevel { get; set; } = 0;
		public int WeaponLevel { get; set; } = 0;
		public int WeaponInitExp { get; set; } = 0;
		public int WeaponFinalExp { get; set; } = 0;
		public int TankExp { get; set; } = 0;
		public int TankInitExp { get; set; } = 0;
		public int TankFinalExp { get; set; } = 0;
		public int ItemsExpDelta { get; set; } = 0;
		public int ContainerScore { get; set; } = 0;
		public int ContainerScoreDelta { get; set; } = 0;
		public int ContainerScoreLimit { get; set; } = 0;
        public Entity Container => _player.Data.League.GetComponent<ChestBattleRewardComponent>().Chest;
		public float ContainerScoreMultiplier { get; set; } = 0;
		public double Reputation { get; set; } = 0;
		public double ReputationDelta { get; set; } = 0;
		public Entity League { get; set; }
		public Entity PrevLeague { get; set; }
		public int LeaguePlace { get; set; } = 1;
		[OptionalMapped]
		public Entity Reward { get; set; }
	}
}
