using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1514369648926L)]
	public class ElevatedAccessUserAddScoreEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.MatchPlayers.Contains(player.BattleLobbyPlayer));

			battle.UpdateUserStatistics(player, additiveScore:Count, additiveKills:0, additiveKillAssists:0, additiveDeath:0);
		}
		public int Count { get; set; }
	}
}