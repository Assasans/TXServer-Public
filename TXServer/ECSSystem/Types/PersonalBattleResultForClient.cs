using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.BattleRewards;
using TXServer.ECSSystem.Components.Item.Tank;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Types
{
	public class PersonalBattleResultForClient
	{
        public PersonalBattleResultForClient(Player player)
        {
            _player = player;

            PrevLeague = _player.Data.League;
            UserTeamColor = player.User.GetComponent<TeamColorComponent>().TeamColor;

            WeaponFinalExp = WeaponUserItem.GetComponent<ExperienceToLevelUpItemComponent>()
                .FinalLevelExperience;
        }

        public void FinalizeResult()
        {
            if (MatchPlayer.Battle.IsMatchMaking)
            {
                if (_player.Data.Statistics.AllBattlesParticipated >= 4)
                    _player.Data.CurrentBattleSeries++;

                // Player experience
                _player.Data.SetExperience(
                    _player.Data.Experience + ScoreWithBonus - MatchPlayer.AlreadyAddedExperience, false);

                // Weapon + Hull rank/experience
                _player.Data.AddHullXp(ScoreWithBonus, _player.CurrentPreset.Hull);
                _player.Data.AddWeaponXp(ScoreWithBonus, _player.CurrentPreset.Weapon);
                _player.CheckTankRankUp(ResourceManager.GetUserItem(_player, _player.CurrentPreset.Hull));
                _player.CheckTankRankUp(ResourceManager.GetUserItem(_player, _player.CurrentPreset.Weapon));

                // Reputation
                // todo: calculate earned reputation
                ReputationDelta = TeamBattleResult == TeamBattleResult.DEFEAT ? -15 : 25;
                _player.Data.Reputation += (int) ReputationDelta;

                // Container reward
                _player.Data.LeagueChestScore += ScoreWithBonus;
                int earnedContainerAmount = (int) Math.Floor((float)_player.Data.LeagueChestScore / ContainerScoreLimit);
                _player.Data.LeagueChestScore -= earnedContainerAmount * ContainerScoreLimit;
                if (earnedContainerAmount > 0)
                    _player.SaveNewMarketItem(Container, earnedContainerAmount);

                // rewards
                Entity reward = Leveling.GetTankRankRewards(_player);
                if (reward is not null)
                {
                    _player.ShareEntities(reward);
                    Reward = reward;
                }

                _player.Data.Statistics.AllBattlesParticipated++;
                _player.Data.Statistics.BattlesParticipated++;
                _player.Data.Statistics.BattlesParticipatedInSeason++;
                _player.Data.Statistics.Deaths = MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().Deaths;
                _player.Data.Statistics.Kills = MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().Kills;

                switch (_player.BattlePlayer.Battle.ModeHandler.TeamBattleResultFor(_player.BattlePlayer))
                {
                    case TeamBattleResult.WIN:
                        _player.Data.Statistics.Victories++;
                        break;
                    case TeamBattleResult.DEFEAT:
                        _player.Data.Statistics.Defeats++;
                        break;
                    case TeamBattleResult.DRAW or _:
                        _player.Data.Statistics.Draws++;
                        break;
                }
            }
            else
            {
                _player.Data.Statistics.AllCustomBattlesParticipated++;
                _player.Data.Statistics.CustomBattlesParticipated++;
            }

            switch (_player.BattlePlayer.Battle.Params.BattleMode)
            {
                case BattleMode.CTF:
                    _player.Data.Statistics.CtfPlayed++;
                    break;
                case BattleMode.TDM:
                    _player.Data.Statistics.TdmPlayed++;
                    break;
                case BattleMode.DM or _:
                    _player.Data.Statistics.DmPlayed++;
                    break;
            }

            _player.User.ChangeComponent<UserStatisticsComponent>();
        }

        private readonly Player _player;
        private MatchPlayer MatchPlayer => _player.BattlePlayer.MatchPlayer;

        private static readonly Dictionary<int, float> BattleSeriesMultipliers = new()
            {{0, 1}, {1, 1.05f}, {2, 1.10f}, {3, 1.15f}, {4, 1.2f}, {5, 1.25f}};

        private int Score => MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses;
        [ProtocolIgnore] public int ScoreWithBonus => (int) (MatchPlayer.GetScoreWithBonus(Score) +
                                                             ScoreBattleSeriesMultiplier / 100 * Score);

        private Entity TankUserItem => ResourceManager.GetUserItem(_player, _player.CurrentPreset.Hull);
        private Entity WeaponUserItem => ResourceManager.GetUserItem(_player, _player.CurrentPreset.Weapon);


        public TeamColor UserTeamColor { get; set; }
		public TeamBattleResult TeamBattleResult =>
            _player.BattlePlayer.Battle.ModeHandler.TeamBattleResultFor(_player.BattlePlayer);

		public int Energy => 0;
		public int EnergyDelta => 0;
		public int CrystalsForExtraEnergy => 0;
		[OptionalMapped] public EnergySource MaxEnergySource { get; set; }

        public int CurrentBattleSeries => _player.Data.CurrentBattleSeries;
        public int MaxBattleSeries => 5;
        public float ScoreBattleSeriesMultiplier =>
            BattleSeriesMultipliers[BattleSeriesMultipliers.Keys.Where(k => k <= CurrentBattleSeries).Max()];

        public int RankExp => (int) _player.Data.Experience;
        public int RankExpDelta => ScoreWithBonus;
		public int WeaponExp => (int) _player.Data.Weapons.GetById(_player.CurrentPreset.Weapon.EntityId).Xp;
        public int TankLevel => _player.GetUserItemLevel(_player.CurrentPreset.Weapon);
        public int WeaponLevel => _player.GetUserItemLevel(_player.CurrentPreset.Weapon);
        public int WeaponInitExp =>
            (int) WeaponUserItem.GetComponent<ExperienceItemComponent>().Experience - ScoreWithBonus;
		public int WeaponFinalExp { get; set; }
		public int TankExp => (int) _player.Data.Hulls.GetById(_player.CurrentPreset.Hull.EntityId).Xp;
		public int TankInitExp => (int) TankUserItem.GetComponent<ExperienceItemComponent>().Experience -
                                  ScoreWithBonus;
		public int TankFinalExp => TankUserItem.GetComponent<ExperienceToLevelUpItemComponent>()
            .FinalLevelExperience;
        public int ItemsExpDelta => ScoreWithBonus;

		public int ContainerScore => (int) _player.Data.LeagueChestScore;
        public int ContainerScoreDelta => ScoreWithBonus;
		public int ContainerScoreLimit => 1000;
        public Entity Container => _player.Data.League.GetComponent<ChestBattleRewardComponent>().Chest;
		public float ContainerScoreMultiplier { get; set; }

        public double Reputation => _player.Data.Reputation;
		public double ReputationDelta { get; set; }
        public Entity League => _player.Data.League;
        public Entity PrevLeague { get; set; }

		public int LeaguePlace { get; set; } = 1;

		[OptionalMapped] public Entity Reward { get; set; }
	}
}
