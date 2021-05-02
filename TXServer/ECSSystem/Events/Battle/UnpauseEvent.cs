using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(-3944419188146485646L)]
	public class UnpauseEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			if (!player.BattlePlayer.MatchPlayer.Paused) return;
			
			player.BattlePlayer.MatchPlayer.Paused = false;
			player.BattlePlayer.MatchPlayer.IdleKickTime = null;
			
			entity.RemoveComponent<PauseComponent>();
			entity.RemoveComponent<IdleCounterComponent>();
		}
	}
}