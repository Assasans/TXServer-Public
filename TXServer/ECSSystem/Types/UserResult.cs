using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Battles;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Item.Module;
using TXServer.ECSSystem.Components.User;

namespace TXServer.ECSSystem.Types
{
    public class UserResult
    {
        public UserResult(BattleTankPlayer battlePlayer, IEnumerable<UserResult> userResults)
        {
            _battlePlayer = battlePlayer;
            _userResults = userResults;

            EnterTime = DateTime.UtcNow.Ticks;

            foreach (Entity module in battlePlayer.Player.CurrentPreset.GetPlayerModules(battlePlayer.Player).Values)
                Modules.Add(new ModuleInfo(module.GetComponent<MarketItemGroupComponent>().Key,
                    module.GetComponent<ModuleUpgradeLevelComponent>().Level));

        }

        private readonly IEnumerable<UserResult> _userResults;
        private readonly BattleTankPlayer _battlePlayer;

        public long UserId => _battlePlayer.User.EntityId;
        public string Uid => _battlePlayer.User.GetComponent<UserUidComponent>().Uid;
        public int Rank => _battlePlayer.User.GetComponent<UserRankComponent>().Rank;
        public string AvatarId => _battlePlayer.User.GetComponent<UserAvatarComponent>().Id;
        public long BattleUserId => _battlePlayer.MatchPlayer.BattleUser.EntityId;
        public double ReputationInBattle => _battlePlayer.Player.Data.Reputation;

        public long EnterTime { get; set; }

        public int Place => _userResults.OrderBy(x => x.ScoreWithoutPremium).ToList().IndexOf(this);
        public int Kills { get; set; } = 0;
        public int KillAssists { get; set; }
        public int KillStrike { get; set; }
        public int Deaths { get; set; }
        public int Damage { get; set; }

        public int Score => _battlePlayer.MatchPlayer.GetScoreWithBonus(ScoreWithoutPremium);
        public int ScoreWithoutPremium => _battlePlayer.MatchPlayer.RoundUser
            .GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses;
        public int ScoreToExperience => _battlePlayer.MatchPlayer.PersonalBattleResult.ScoreWithBonus;

        public int RankExpDelta => (int) _battlePlayer.Player.Data.Experience;

        public int ItemsExpDelta { get; set; } = 0;

        public int Flags { get; set; }
        public int FlagAssists { get; set; }
        public int FlagReturns { get; set; }

        public long WeaponId => _battlePlayer.Player.CurrentPreset.Weapon.EntityId;
        public long HullId => _battlePlayer.Player.CurrentPreset.Hull.EntityId;
        public long PaintId => _battlePlayer.Player.CurrentPreset.HullPaint.EntityId;
        public long CoatingId => _battlePlayer.Player.CurrentPreset.WeaponPaint.EntityId;
        public long HullSkinId => _battlePlayer.Player.CurrentPreset.HullSkin.EntityId;
        public long WeaponSkinId => _battlePlayer.Player.CurrentPreset.WeaponSkin.EntityId;
        public List<ModuleInfo> Modules { get; set; } = new();

        public int BonusesTaken { get; set; } = 0;

        public bool UnfairMatching => _battlePlayer.Battle.JoinedTankPlayers.Count() <= 3 ||
                                      _battlePlayer.Battle.ModeHandler is Battle.TeamBattleHandler tbHandler &&
                                      Math.Abs(tbHandler.RedTeamPlayers.Count - tbHandler.BlueTeamPlayers.Count) >= 2;
        public bool Deserted { get; set; }

        public Entity League => _battlePlayer.Player.Data.League;
    }
}
