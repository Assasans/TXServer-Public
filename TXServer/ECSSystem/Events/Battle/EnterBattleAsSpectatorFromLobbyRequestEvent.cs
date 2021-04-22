using System.Linq;
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
			
			Core.Battles.Battle battle = Server.Instance.FindBattleById(lobbyId: 0, battleId: BattleId);
			foreach (Entity user in battle.AllBattlePlayers.Select(x => x.User))
			{
				if (player.EntityList.Contains(user))
					player.UnshareEntity(user);
			}
			battle.AddPlayer(player, true);
        }

		public long BattleId { get; set; }
	}
}