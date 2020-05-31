using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

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
			}
		}

		public string Message { get; set; }
	}
}
