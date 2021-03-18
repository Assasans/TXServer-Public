using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1504069546662L)]
	public class ElevatedAccessUserIncreaseScoreEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			Core.Battles.Battle battle = player.BattlePlayer.Battle;

            if (battle.ModeHandler is not TeamBattleHandler handler) return;

            BattleView teamView = handler.BattleViewFor(player.BattlePlayer);
			Entity team = TeamColor == TeamColor.BLUE ? teamView.AllyTeamEntity : teamView.EnemyTeamEntity;

			battle.UpdateScore(player, team, Amount);
		}
		public TeamColor TeamColor { get; set; }
		public int Amount { get; set; }
	}
}