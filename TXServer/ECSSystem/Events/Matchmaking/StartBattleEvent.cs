using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Core;
using System.Linq;
using TXServer.Core.Battles;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1497356545125L)]
	public class StartBattleEvent : ECSEvent
	{
		public void Execute(Player player, Entity toOpenBattleLobby)
		{ 
		    Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.Owner == player);

			if (battle.BattleState == BattleState.CustomNotStarted)
            {
				battle.CountdownTimer = 3;
				battle.BattleLobbyEntity.AddComponent(new MatchMakingLobbyStartingComponent());
				battle.BattleState = BattleState.Starting;
            }
			else
            {
				battle.InitBattlePlayer(player.BattleLobbyPlayer);
			}
		}
	}
}