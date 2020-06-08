using System.Collections.Generic;
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
		public void Execute(Entity chat)
        {
			SendEventCommand command = new SendEventCommand(new ChatMessageReceivedEvent
			{
				Message = this.Message,
				SystemMessage = false,
				UserId = Player.Instance.User.EntityId,
				UserUid = Player.Instance.User.GetComponent<UserUidComponent>().Uid,
				UserAvatarId = Player.Instance.User.GetComponent<UserAvatarComponent>().Id
			}, chat);

			switch (chat.TemplateAccessor.Template)
			{
				case GeneralChatTemplate _:
					foreach (Player player in ServerLauncher.Pool)
					{
						player.LobbyCommandQueue.Enqueue(() => CommandManager.SendCommands(player.Socket, command));
					}
					break;
				case PersonalChatTemplate _:
					Entity sender = Player.Instance.User;

					foreach (Entity user in chat.GetComponent<ChatParticipantsComponent>().Users)
                    {
						user.Owner.LobbyCommandQueue.Enqueue(() =>
						{
							lock (Player.Instance.EntityList)
							{
								List<Command> commands = new List<Command>();

								bool isShared = false;

								if (!user.Owner.EntityList.Contains(sender))
								{
									commands.Add(new EntityShareCommand(sender));
									isShared = true;
								}

								if (!user.Owner.EntityList.Contains(chat))
									commands.Add(new EntityShareCommand(chat));

								commands.Add(command);

								if (isShared)
									commands.Add(new EntityUnshareCommand(sender));


								CommandManager.SendCommands(Player.Instance.Socket, commands);
							}
						});
					}
					break;
			}
		}

		public string Message { get; set; }
	}
}
