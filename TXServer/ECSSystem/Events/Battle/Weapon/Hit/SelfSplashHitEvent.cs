using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Hit
{
    [SerialVersionUID(196833391289212110L)]
	public class SelfSplashHitEvent : SelfHitEvent
	{
        public new void Execute(Player player, Entity weapon)
		{
            SelfEvent.Execute(this, player, weapon);

            MatchPlayer matchPlayer = player.BattlePlayer.MatchPlayer;

            foreach (HitTarget hitTarget in Targets.Where(hitTarget =>
                matchPlayer.Battle.Params.FriendlyFire ||
                matchPlayer.IsEnemyOf(Damage.GetTargetByHit(matchPlayer, hitTarget))))
                Damage.HandleHit(weapon, player.BattlePlayer.MatchPlayer, hitTarget);

            foreach (HitTarget splashTarget in SplashTargets.Where(hitTarget =>
                matchPlayer.Battle.Params.FriendlyFire ||
                matchPlayer.IsEnemyOf(Damage.GetTargetByHit(matchPlayer, hitTarget)) ||
                matchPlayer.BattleWeapon.AllowsSelfDamage && hitTarget.Entity == player.BattlePlayer.MatchPlayer.Tank))
                Damage.HandleHit(weapon, player.BattlePlayer.MatchPlayer, splashTarget, true);

            int hitCount = Targets.Concat(SplashTargets).Distinct().Count();

            player.Data.Statistics.Shots++;
            player.Data.Statistics.Hits += hitCount;
            player.User.ChangeComponent<UserStatisticsComponent>();
        }

		public override IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteSplashHitEvent>();

        [OptionalMapped] public List<HitTarget> SplashTargets { get; set; }
	}
}
