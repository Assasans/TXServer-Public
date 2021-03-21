using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1506087128828L)]
	public class ElevatedAccessUserFinishBattleEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			if (player.BattlePlayer != null)
			    player.BattlePlayer.Battle.FinishBattle();
			else
            {
				foreach (Core.Battles.Battle loopedBattle in ServerConnection.BattlePool)
					loopedBattle.FinishBattle();
			}
		}
	}
}