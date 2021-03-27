using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
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
				BattlePlayer hitPlayer = battle.MatchPlayers.Single(p => p.MatchPlayer.Incarnation == hitTarget.IncarnationEntity);

				if (hitPlayer.Player.User.GetComponent<TeamColorComponent>().TeamColor == player.User.GetComponent<TeamColorComponent>().TeamColor && !battle.Params.FriendlyFire)
					return;

				hitPlayer.MatchPlayer.DealDamage(player, hitTarget, 900);
			}

			foreach (HitTarget splashTarget in SplashTargets)
			{
				BattlePlayer hitPlayer = battle.MatchPlayers.Single(p => p.MatchPlayer.Incarnation == splashTarget.IncarnationEntity);

				if (hitPlayer.Player.User.GetComponent<TeamColorComponent>().TeamColor == player.User.GetComponent<TeamColorComponent>().TeamColor)
                {
					if (hitPlayer.Player != player && !battle.Params.FriendlyFire)
						return;
				}
					

				hitPlayer.MatchPlayer.DealDamage(player, splashTarget, 500);
			}
		}

		public override IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteSplashHitEvent>();

        [OptionalMapped]
		public List<HitTarget> SplashTargets { get; set; }
	}
}