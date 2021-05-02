using System;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.Core.Battles
{
    public class BattleTankPlayer : BaseBattlePlayer
    {
        public BattleTankPlayer(Battle battle, Player player, Entity team)
        {
            Battle = battle;
            Player = player;
            User = player.User;
            Team = team;
        }

        public override void Reset()
        {
            base.Reset();
            MatchPlayer = null;
            User.TryRemoveComponent<MatchMakingUserReadyComponent>();
        }

        public Entity Team { get; set; }
        public MatchPlayer MatchPlayer { get; set; }

        public DateTime MatchMakingJoinCountdown { get; set; } = DateTime.Now.AddSeconds(10);

        public float? BulletSpeed { get; set; }
        public float? TurretKickback { get; set; }
        public float? TurretRotationSpeed { get; set; }
        public float? TurretUnloadEnergyPerShot { get; set; }
        public bool IsCheatImmune { get; set; }
    }
}
