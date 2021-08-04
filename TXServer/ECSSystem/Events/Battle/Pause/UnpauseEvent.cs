using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;

namespace TXServer.ECSSystem.Events.Battle.Pause
{
	[SerialVersionUID(-3944419188146485646L)]
	public class UnpauseEvent : ECSEvent
	{
		public void Execute(Player player, Entity user)
		{
            player.BattlePlayer.MatchPlayer.Paused = false;
			player.BattlePlayer.MatchPlayer.IdleKickTime = null;

			user.TryRemoveComponent<PauseComponent>();
			user.TryRemoveComponent<IdleCounterComponent>();
		}
	}
}
