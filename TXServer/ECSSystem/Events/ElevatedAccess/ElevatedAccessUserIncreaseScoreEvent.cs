using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
	[SerialVersionUID(1504069546662L)]
	public class ElevatedAccessUserIncreaseScoreEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
            if (!player.Data.Admin) return;

			if (player.BattlePlayer?.Battle.ModeHandler is not TeamBattleHandler handler) return;

            BattleView teamView = handler.BattleViewFor(player.BattlePlayer);
			Entity team = TeamColor == TeamColor.BLUE ? teamView.AllyTeamEntity : teamView.EnemyTeamEntity;

			player.BattlePlayer.Battle.UpdateScore(team, Amount);

			if (player.BattlePlayer.Battle.Params.BattleMode == BattleMode.CTF)
			{
				Flag flag = TeamColor == TeamColor.BLUE ? teamView.AllyTeamFlag : teamView.EnemyTeamFlag;
				player.BattlePlayer.Battle.PlayersInMap.SendEvent(new FlagDeliveryEvent(), flag.FlagEntity);
			}
		}
		public TeamColor TeamColor { get; set; }
		public int Amount { get; set; }
	}
}
