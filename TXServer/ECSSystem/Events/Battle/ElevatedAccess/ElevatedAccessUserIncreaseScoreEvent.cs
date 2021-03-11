using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1504069546662L)]
	public class ElevatedAccessUserIncreaseScoreEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.MatchPlayers.Contains(player.BattleLobbyPlayer));
			Entity[] teams = { battle.RedTeamEntity, battle.BlueTeamEntity };
			Entity team;

			if (TeamColor == TeamColor.BLUE)
			    team = teams.Single(t => t.GetComponent<TeamColorComponent>().TeamColor == player.User.GetComponent<TeamColorComponent>().TeamColor);
			else
				team = teams.Single(t => t.GetComponent<TeamColorComponent>().TeamColor != player.User.GetComponent<TeamColorComponent>().TeamColor);

			battle.UpdateScore(player, team, Amount);
		}
		public TeamColor TeamColor { get; set; }
		public int Amount { get; set; }
	}
}