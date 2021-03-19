using TXServer.Core;
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
			// double check needed
			if (player.BattlePlayer.MatchPlayer.Paused)
				entity.RemoveComponent<PauseComponent>();
			
			player.BattlePlayer.MatchPlayer.Paused = false;
		}
	}
}