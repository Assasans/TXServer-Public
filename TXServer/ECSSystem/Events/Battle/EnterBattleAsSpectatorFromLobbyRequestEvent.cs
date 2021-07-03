using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1498554483631L)]
	public class EnterBattleAsSpectatorFromLobbyRequestEvent : ECSEvent
	{
		public void Execute(Player player, Entity battleUser)
		{
			if (player.IsInSquad || player.IsInBattle) return;
			
			Server.Instance.FindBattleById(battleId: BattleId).AddPlayer(player, true);
        }

		public long BattleId { get; set; }
	}
}