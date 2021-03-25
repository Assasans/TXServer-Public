using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using System.Linq;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1446035600297L)]
	public class SendChatMessageEvent : ECSEvent
	{
		public void Execute(Player player, Entity chat)
        {
			Core.Battles.Battle battle = player.BattlePlayer?.Battle;

			ChatMessageReceivedEvent evt = new()
			{
				Message = Message,
				SystemMessage = false,
				UserId = player.User.EntityId,
				UserUid = player.User.GetComponent<UserUidComponent>().Uid,
				UserAvatarId = player.User.GetComponent<UserAvatarComponent>().Id
			};

			string reply = ChatCommands.CheckForCommand(player, Message);
			if (!string.IsNullOrEmpty(reply))
            {
				evt.Message = reply;
				evt.SystemMessage = true;
				player.SendEvent(evt, chat);
				return;
            }

			// todo: return if user has chat ban

			switch (chat.TemplateAccessor.Template)
			{
				case GeneralChatTemplate _:
					player.Server.Connection.Pool.SendEvent(evt, chat);
					break;
				case PersonalChatTemplate _:
					foreach (Player p in chat.GetComponent<ChatParticipantsComponent>().GetPlayers())
                    {
						bool isShared = false;
						if (!p.EntityList.Contains(chat))
                        {
							if (!p.EntityList.Contains(player.User))
							{
								p.ShareEntity(player.User);
								isShared = true;
							}

							p.ShareEntity(chat);
                        }
						p.SendEvent(evt, chat);

						if (isShared)
							p.UnshareEntity(player.User);
					}
					break;
				case BattleLobbyChatTemplate _:
				case GeneralBattleChatTemplate _:
					battle.AllBattlePlayers.Select(p => p.Player).SendEvent(evt, chat);
					break;
				case TeamBattleChatTemplate _:
					battle.AllBattlePlayers.Where(x => x.Team == player.BattlePlayer.Team).Select(p => p.Player).SendEvent(evt, chat);
					break;
			}
		}

		public string Message { get; set; }
	}
}
