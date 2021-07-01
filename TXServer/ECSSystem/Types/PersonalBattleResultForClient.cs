using System.Collections.Generic;
using System.Linq;
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
            if (MatchPlayer.Battle.IsMatchMaking)
            {
                _player.Data.CurrentBattleSeries++;

                // Rank
                _player.Data.SetExperience(_player.Data.Experience + ScoreWithBonus, false);
                RankExp = (int) _player.Data.Experience;
                RankExpDelta = ScoreWithBonus;

                // Reputation
                // todo: calculate earned reputation
                ReputationDelta = TeamBattleResult == TeamBattleResult.DEFEAT ? -15 : 25;
                _player.Data.Reputation += (int) ReputationDelta;

                // Container reward
                ContainerScoreDelta = ScoreWithBonus;
                _player.Data.LeagueChestScore += ContainerScoreDelta;


                _player.User.ChangeComponent<UserStatisticsComponent>(component =>
                {
                    component.Statistics["ALL_BATTLES_PARTICIPATED"]++;
                    component.Statistics["BATTLES_PARTICIPATED"]++;
                    component.Statistics["BATTLES_PARTICIPATED_IN_SEASON"]++;
                    component.Statistics["DEATHS"] =
                        MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().Deaths;
                    component.Statistics["KILLS"] =
                        MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().Kills;
                });
            }
            else
            {
                _player.User.ChangeComponent<UserStatisticsComponent>(component =>
                {
                    component.Statistics["ALL_CUSTOM_BATTLES_PARTICIPATED"]++;
                    component.Statistics["CUSTOM_BATTLES_PARTICIPATED"]++;
                });
            }

            _player.User.ChangeComponent<UserStatisticsComponent>(component =>
            {
                component.Statistics[$"{MatchPlayer.Battle.Params.BattleMode}_PLAYED"]++;
            });
        }

        private readonly Player _player;
        private MatchPlayer MatchPlayer => _player.BattlePlayer.MatchPlayer;

        private static readonly Dictionary<int, int> BattleSeriesMultipliers = new()
            {{1, 5}, {2, 10}, {3, 15}, {4, 20}, {5, 25}};

        private int Score => MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses;
        private int ScoreWithBonus =>
            (int) (MatchPlayer.GetScoreWithPremium(Score) + ScoreBattleSeriesMultiplier / 100 * Score);

        public TeamColor UserTeamColor { get; set; }
		public TeamBattleResult TeamBattleResult =>
            _player.BattlePlayer.Battle.ModeHandler.TeamBattleResultFor(_player.BattlePlayer);

		public int Energy { get; set; } = 0;
		public int EnergyDelta { get; set; } = 0;
		public int CrystalsForExtraEnergy { get; set; } = 0;
		[OptionalMapped]
		public EnergySource MaxEnergySource { get; set; } = EnergySource.MVP_BONUS;

        public int CurrentBattleSeries => _player.Data.CurrentBattleSeries;
        public int MaxBattleSeries => 5;
        public float ScoreBattleSeriesMultiplier =>
            BattleSeriesMultipliers[BattleSeriesMultipliers.Keys.Where(k => k <= CurrentBattleSeries).Max()];

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
