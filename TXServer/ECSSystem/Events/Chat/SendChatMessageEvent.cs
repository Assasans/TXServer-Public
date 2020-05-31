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
					foreach (Entity user in chat.GetComponent<ChatParticipantsComponent>().Users)
                    {
						bool isShared = false;
						if (!user.Owner.EntityList.Contains(chat))
                        {
							if (!user.Owner.EntityList.Contains(player.User))
							{
								CommandManager.SendCommands(user.Owner, new EntityShareCommand(player.User));
								isShared = true;
							}

							CommandManager.SendCommands(user.Owner, new EntityShareCommand(chat));
                        }
						CommandManager.SendCommands(user.Owner, command);

						if (isShared)
							CommandManager.SendCommands(user.Owner, new EntityUnshareCommand(Player.Instance.User));
					}
					break;
			}
		}

		public string Message { get; set; }
	}
}
