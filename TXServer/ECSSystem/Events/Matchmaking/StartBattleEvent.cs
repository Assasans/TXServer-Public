using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Core;
using System.Linq;
using TXServer.Core.Battles;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1497356545125L)]
	public class StartBattleEvent : ECSEvent
	{
		public void Execute(Player player, Entity toOpenBattleLobby)
		{ 
		    Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.Owner == player);

			if (battle.BattleState == BattleState.CustomNotStarted)
				battle.BattleState = BattleState.Starting;
			else if (battle.BattleState == BattleState.Ended)
            {
				battle.CreateBattle();
				battle.BattleState = BattleState.Starting;
			}
			else
				battle.InitBattlePlayer(player.BattleLobbyPlayer);
		}
	}
}