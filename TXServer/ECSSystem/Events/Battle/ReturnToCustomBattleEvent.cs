using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1498743823980L)]
	public class ReturnToCustomBattleEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.AllBattlePlayers.Contains(player.BattleLobbyPlayer));
			battle.InitBattlePlayer(player.BattleLobbyPlayer);
		}
	}
}