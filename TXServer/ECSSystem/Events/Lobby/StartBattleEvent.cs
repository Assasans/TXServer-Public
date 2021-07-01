using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Lobby
{
    [SerialVersionUID(1497356545125L)]
	public class StartBattleEvent : ECSEvent
	{
		public void Execute(Player player, Entity toOpenBattleLobby)
		{
		    Core.Battles.Battle battle = player.BattlePlayer.Battle;

			if (battle.BattleState == BattleState.CustomNotStarted)
				battle.BattleState = BattleState.Starting;
			else if (battle.BattleState == BattleState.Ended)
            {
				battle.CreateBattle();
				battle.BattleState = BattleState.Starting;
			}
			else
            {
				battle.InitMatchPlayer(player.BattlePlayer);
			}
		}
	}
}
