using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using System.Collections.Generic;
using System.Linq;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1446035600297L)]
	public class SendChatMessageEvent : ECSEvent
	{
		public void Execute(Player player, Entity chat)
        {
			SendEventCommand command = new SendEventCommand(new ChatMessageReceivedEvent
			{
				Message = this.Message,
				SystemMessage = false,
				UserId = player.User.EntityId,
				UserUid = player.User.GetComponent<UserUidComponent>().Uid,
				UserAvatarId = player.User.GetComponent<UserAvatarComponent>().Id
			}, chat);

			switch (chat.TemplateAccessor.Template)
			{
				case GeneralChatTemplate _:
					foreach (Player p in player.Server.Connection.Pool)
					{
						CommandManager.SendCommands(p, command);
					}
					break;
				case PersonalChatTemplate _:
					foreach (Player p in chat.GetComponent<ChatParticipantsComponent>().GetPlayers())
                    {
						bool isShared = false;
						if (!p.EntityList.Contains(chat))
                        {
							if (!p.EntityList.Contains(player.User))
							{
								CommandManager.SendCommands(p, new EntityShareCommand(player.User));
								isShared = true;
							}

							CommandManager.SendCommands(p, new EntityShareCommand(chat));
                        }
						CommandManager.SendCommands(p, command);

						if (isShared)
							CommandManager.SendCommands(p, new EntityUnshareCommand(player.User));
					}
					break;
				case BattleLobbyChatTemplate _:
					Core.Battles.Battle battleLobby = ServerConnection.BattlePool.Single(b => b.BlueTeamPlayers.Concat(b.RedTeamPlayers).Concat(b.DMTeamPlayers).Contains(player.BattleLobbyPlayer));
					
					foreach (BattleLobbyPlayer p in battleLobby.BlueTeamPlayers.Concat(battleLobby.RedTeamPlayers).Concat(battleLobby.DMTeamPlayers))
                    {
						CommandManager.SendCommands(p.Player, command);
					}
					break;
				case GeneralBattleChatTemplate _:
					Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.BlueTeamPlayers.Concat(b.RedTeamPlayers).Concat(b.DMTeamPlayers).Contains(player.BattleLobbyPlayer));

					foreach (BattleLobbyPlayer p in battle.BlueTeamPlayers.Concat(battle.RedTeamPlayers).Concat(battle.DMTeamPlayers))
                    {
						CommandManager.SendCommands(p.Player, command);
					}
					break;
				case TeamBattleChatTemplate _:
					Core.Battles.Battle teamBattle = ServerConnection.BattlePool.Single(b => b.BlueTeamPlayers.Concat(b.RedTeamPlayers).Contains(player.BattleLobbyPlayer));
					List<BattleLobbyPlayer> team;

					if (player.BattleLobbyPlayer.Team.GetComponent<TeamGroupComponent>().Key == teamBattle.BlueTeamEntity.GetComponent<TeamGroupComponent>().Key)
                    {
						team = teamBattle.BlueTeamPlayers;
                    } 
					else
                    {
						team = teamBattle.RedTeamPlayers;
					}
					
					foreach (BattleLobbyPlayer p in team)
					{
						CommandManager.SendCommands(p.Player, command);
					}
					break;
			}
		}

		public string Message { get; set; }
	}
}
