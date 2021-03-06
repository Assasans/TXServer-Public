using System;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;

namespace TXServer.ECSSystem.Events.Battle.Pause
{
	[SerialVersionUID(-1316093147997460626L)]
	public class PauseEvent : ECSEvent
	{
		public void Execute(Player player, Entity user)
		{
			if (player.BattlePlayer.MatchPlayer.Paused ||
                player.IsInMatch && player.BattlePlayer.Battle.SuppressInactivityKick) return;

			player.BattlePlayer.MatchPlayer.Paused = true;
			player.BattlePlayer.MatchPlayer.IdleKickTime = DateTime.UtcNow.AddMinutes(2);

			player.BattlePlayer.MatchPlayer.BattleUser.AddComponent(new PauseComponent());
			player.BattlePlayer.MatchPlayer.BattleUser.AddComponent(new IdleCounterComponent(0));
			player.SendEvent(new IdleBeginTimeSyncEvent(DateTime.UtcNow), player.BattlePlayer.MatchPlayer.BattleUser);
		}
	}
}
