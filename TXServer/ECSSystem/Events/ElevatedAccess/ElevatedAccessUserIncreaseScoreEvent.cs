using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
	[SerialVersionUID(1504069546662L)]
	public class ElevatedAccessUserIncreaseScoreEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			if (player.BattlePlayer?.Battle.ModeHandler is not TeamBattleHandler handler) return;

            BattleView teamView = handler.BattleViewFor(player.BattlePlayer);
			Entity team = TeamColor == TeamColor.BLUE ? teamView.AllyTeamEntity : teamView.EnemyTeamEntity;

			player.BattlePlayer.Battle.UpdateScore(team, Amount);
		}
		public TeamColor TeamColor { get; set; }
		public int Amount { get; set; }
	}
}