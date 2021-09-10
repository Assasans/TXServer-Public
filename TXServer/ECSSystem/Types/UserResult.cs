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
        public UserResult(BattleTankPlayer battlePlayer)
        {
            _battlePlayer = battlePlayer;

            EnterTime = DateTime.UtcNow.Ticks;

            foreach (Entity module in battlePlayer.Player.CurrentPreset.Modules.Values.Where(module => module != null))
                Modules.Add(new ModuleInfo(module.GetComponent<MarketItemGroupComponent>().Key,
                    module.GetComponent<ModuleUpgradeLevelComponent>().Level));

        }

        private IEnumerable<UserResult> UserResults =>
            (_battlePlayer.Battle.ModeHandler as Core.Battles.Battle.TeamBattleHandler)?.BattleViewFor(_battlePlayer)
            .AllyTeamResults ?? ((Core.Battles.Battle.SoloBattleHandler) _battlePlayer.Battle.ModeHandler).Results;
        private readonly BattleTankPlayer _battlePlayer;

        public long UserId => _battlePlayer.User.EntityId;
        public string Uid => _battlePlayer.User.GetComponent<UserUidComponent>().Uid;
        public int Rank => _battlePlayer.User.GetComponent<UserRankComponent>().Rank;
        public string AvatarId => _battlePlayer.User.GetComponent<UserAvatarComponent>().Id;
        public long BattleUserId => _battlePlayer.MatchPlayer.BattleUser.EntityId;
        public double ReputationInBattle => _battlePlayer.Player.Data.Reputation;

        public long EnterTime { get; set; }

        public int Place => UserResults.OrderBy(x => x.ScoreWithoutPremium).ToList().IndexOf(this);
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

        public long WeaponId => _battlePlayer.Player.CurrentPreset.Weapon.GetComponent<MarketItemGroupComponent>().Key;
        public long HullId => _battlePlayer.Player.CurrentPreset.Hull.GetComponent<MarketItemGroupComponent>().Key;
        public long PaintId => _battlePlayer.Player.CurrentPreset.TankPaint.GetComponent<MarketItemGroupComponent>().Key;
        public long CoatingId => _battlePlayer.Player.CurrentPreset.WeaponPaint.GetComponent<MarketItemGroupComponent>().Key;
        public long HullSkinId => _battlePlayer.Player.CurrentPreset
            .HullSkins[_battlePlayer.Player.CurrentPreset.HullItem].GetComponent<MarketItemGroupComponent>().Key;
        public long WeaponSkinId => _battlePlayer.Player.CurrentPreset
            .WeaponSkins[_battlePlayer.Player.CurrentPreset.WeaponItem].GetComponent<MarketItemGroupComponent>().Key;
        public List<ModuleInfo> Modules { get; set; } = new();

        public int BonusesTaken { get; set; } = 0;

        public bool UnfairMatching => _battlePlayer.Battle.JoinedTankPlayers.Count() <= 3 ||
                                      _battlePlayer.Battle.ModeHandler is Core.Battles.Battle.TeamBattleHandler tbHandler &&
                                      Math.Abs(tbHandler.RedTeamPlayers.Count - tbHandler.BlueTeamPlayers.Count) >= 2;
        public bool Deserted { get; set; }

        public Entity League => _battlePlayer.Player.Data.League;
    }
}
