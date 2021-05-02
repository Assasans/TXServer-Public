using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Battles;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Types
{
    public class UserResult
    {
        public UserResult(BattleTankPlayer battlePlayer, IEnumerable<UserResult> userResults)
        {
            BattlePlayer = battlePlayer;
            UserResults = userResults;

            Entity user = BattlePlayer.User;
            UserId = user.EntityId;
            Uid = user.GetComponent<UserUidComponent>().Uid;
            Rank = user.GetComponent<UserRankComponent>().Rank;
            AvatarId = user.GetComponent<UserAvatarComponent>().Id;
            EnterTime = DateTime.UtcNow.Ticks;
            foreach (Entity module in battlePlayer.Player.CurrentPreset.Modules.Values)
            {
                if (module != null)
                    Modules.Add(new ModuleInfo(module.GetComponent<MarketItemGroupComponent>().Key, module.GetComponent<ModuleUpgradeLevelComponent>().Level));
            }
            League = Leagues.GlobalItems.Silver;
        }

        private readonly IEnumerable<UserResult> UserResults;
        private readonly BattleTankPlayer BattlePlayer;

        public long UserId { get; set; }
        public string Uid { get; set; }
        public int Rank { get; set; }
        public string AvatarId { get; set; }
        public long BattleUserId => BattlePlayer.MatchPlayer.BattleUser.EntityId;
        public double ReputationInBattle { get; set; } = 0;
        public long EnterTime { get; set; }
        public int Place => UserResults.OrderBy(x => x.ScoreWithoutPremium).ToList().IndexOf(this);
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
        public int Flags { get; set; } = 0;
        public int FlagAssists { get; set; } = 0;
        public int FlagReturns { get; set; } = 0;
        public long WeaponId => BattlePlayer.Player.CurrentPreset.Weapon.GetComponent<MarketItemGroupComponent>().Key;
        public long HullId => BattlePlayer.Player.CurrentPreset.Hull.GetComponent<MarketItemGroupComponent>().Key;
        public long PaintId => BattlePlayer.Player.CurrentPreset.TankPaint.GetComponent<MarketItemGroupComponent>().Key;
        public long CoatingId => BattlePlayer.Player.CurrentPreset.WeaponPaint.GetComponent<MarketItemGroupComponent>().Key;
        public long HullSkinId => BattlePlayer.Player.CurrentPreset.HullSkins[BattlePlayer.Player.CurrentPreset.HullItem].GetComponent<MarketItemGroupComponent>().Key;
        public long WeaponSkinId => BattlePlayer.Player.CurrentPreset.WeaponSkins[BattlePlayer.Player.CurrentPreset.WeaponItem].GetComponent<MarketItemGroupComponent>().Key;
        public List<ModuleInfo> Modules { get; set; } = new List<ModuleInfo>() { };
        public int BonusesTaken { get; set; } = 0;
        public bool UnfairMatching { get; set; }
        public bool Deserted { get; set; }
        public Entity League { get; set; }
    }
}
