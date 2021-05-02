using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(196833391289212110L)]
	public class SelfSplashHitEvent : SelfHitEvent, ISelfEvent
	{
		public void Execute(Player player, Entity weapon)
		{
			if (player.BattlePlayer.MatchPlayer.TankState == TankState.Dead)
				return;

			Core.Battles.Battle battle = player.BattlePlayer.Battle;
			
			foreach (HitTarget hitTarget in Targets)
			{
				BattlePlayer victim = battle.MatchPlayers.Single(p => p.MatchPlayer.Incarnation == hitTarget.IncarnationEntity);

				if (player.BattlePlayer.Battle.Params.BattleMode != BattleMode.DM &&
				    victim.Team == player.BattlePlayer.Team && !battle.Params.FriendlyFire)
					return;

				Damage.DealDamage(victim.MatchPlayer, player.BattlePlayer.MatchPlayer, hitTarget, 900);
			}

			foreach (HitTarget splashTarget in SplashTargets)
			{
				BattlePlayer hitPlayer = battle.MatchPlayers.Single(p => p.MatchPlayer.Incarnation == splashTarget.IncarnationEntity);

				if (player.BattlePlayer.Battle.Params.BattleMode != BattleMode.DM &&
				    hitPlayer.Team == player.BattlePlayer.Team && hitPlayer != player.BattlePlayer &&
				    !battle.Params.FriendlyFire)
					return;

				Damage.DealDamage(hitPlayer.MatchPlayer, player.BattlePlayer.MatchPlayer, splashTarget, 500);
			}
		}

		public override IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteSplashHitEvent>();

        [OptionalMapped]
		public List<HitTarget> SplashTargets { get; set; }
	}
}