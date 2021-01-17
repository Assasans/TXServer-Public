using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1499172594697L)]
	public class SwitchTeamEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			foreach (Core.Battles.Battle battle in ServerConnection.BattlePool)
            {
				if (battle.BattleParams.BattleMode != BattleMode.DM)
                {
					if (battle.RedTeamPlayers.Contains(player.BattleLobbyPlayer))
					{
						battle.RedTeamPlayers.Remove(player.BattleLobbyPlayer);
						player.BattleLobbyPlayer = new BattleLobbyPlayer(player, battle.BlueTeamEntity);
						battle.BlueTeamPlayers.Add(player.BattleLobbyPlayer);
						CommandManager.SendCommands(player, 
							new ComponentRemoveCommand(player.User, typeof(TeamColorComponent)),
							new ComponentAddCommand(player.User, new TeamColorComponent(TeamColor.BLUE)));
						break;
					}
					if (battle.BlueTeamPlayers.Contains(player.BattleLobbyPlayer))
					{
						battle.BlueTeamPlayers.Remove(player.BattleLobbyPlayer);
						player.BattleLobbyPlayer = new BattleLobbyPlayer(player, battle.RedTeamEntity);
						battle.RedTeamPlayers.Add(player.BattleLobbyPlayer);
						CommandManager.SendCommands(player, 
							new ComponentRemoveCommand(player.User, typeof(TeamColorComponent)),
							new ComponentAddCommand(player.User, new TeamColorComponent(TeamColor.RED)));
						break;
					}
				}
            }
			
		}
		
	}
}