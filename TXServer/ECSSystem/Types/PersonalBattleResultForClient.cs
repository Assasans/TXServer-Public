using System;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.User;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Types
{
	public class PersonalBattleResultForClient
	{
        public PersonalBattleResultForClient(Player player)
        {
            _player = player;

            PrevLeague = _player.Data.League;
            Reward = BattleRewards.GlobalItems.XCrystalBonus;
			UserTeamColor = player.User.GetComponent<TeamColorComponent>().TeamColor;
        }

        public void FinalizeResult()
        {
            TeamBattleResult = _player.BattlePlayer.Battle.ModeHandler.TeamBattleResultFor(_player.BattlePlayer);

            if (MatchPlayer.Battle.IsMatchMaking)
            {
                // Rank
                _player.Data.SetExperience(_player.Data.Experience +
                                           MatchPlayer.GetScoreWithPremium(MatchPlayer.UserResult.ScoreWithoutPremium -
                                                                           MatchPlayer.AlreadyAddedExperience));
                RankExp = (int) _player.Data.Experience;
                RankExpDelta = MatchPlayer.GetScoreWithPremium(
                    MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses);

                // Reputation
                // todo: calculate earned reputation
                ReputationDelta = TeamBattleResult == TeamBattleResult.DEFEAT ? -15 : 25;
                _player.Data.Reputation += (int) ReputationDelta;

                // Container reward
                ContainerScoreDelta = MatchPlayer.GetScoreWithPremium(
                    MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses);
                _player.Data.LeagueChestScore += ContainerScoreDelta;
            }
        }

        private readonly Player _player;
        private MatchPlayer MatchPlayer => _player.BattlePlayer.MatchPlayer;


        public TeamColor UserTeamColor { get; set; }
		public TeamBattleResult TeamBattleResult { get; set; }
		public int Energy { get; set; } = 0;
		public int EnergyDelta { get; set; } = 0;
		public int CrystalsForExtraEnergy { get; set; } = 0;
		[OptionalMapped]
		public EnergySource MaxEnergySource { get; set; } = EnergySource.MVP_BONUS;

        public int CurrentBattleSeries { get; set; }
		public int MaxBattleSeries { get; set; }
		public float ScoreBattleSeriesMultiplier { get; set; }
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
		public int ContainerScore => (int) _player.User.GetComponent<GameplayChestScoreComponent>().Current;
		public int ContainerScoreDelta { get; set; }
		public int ContainerScoreLimit => (int) _player.User.GetComponent<GameplayChestScoreComponent>().Limit;
        public Entity Container => _player.Data.League.GetComponent<ChestBattleRewardComponent>().Chest;
		public float ContainerScoreMultiplier { get; set; }

        public double Reputation => _player.Data.Reputation;
		public double ReputationDelta { get; set; }
        public Entity League => _player.Data.League;
        public Entity PrevLeague { get; set; }

		public int LeaguePlace { get; set; } = 1;

		[OptionalMapped]
		public Entity Reward { get; set; }
	}
}
