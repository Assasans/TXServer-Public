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
			Core.Battles.Battle battle = Server.FindBattleById(lobbyId: 0, battleId: BattleId);

			// todo: enter battle as spectator system
        }

		public long BattleId { get; set; }
	}
}