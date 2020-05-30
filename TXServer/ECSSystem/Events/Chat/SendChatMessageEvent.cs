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
						player.LobbyCommandQueue.Enqueue(command);
					}
					break;
				case PersonalChatTemplate _:
					foreach (Entity user in chat.GetComponent<ChatParticipantsComponent>().Users)
                    {
						bool isShared = false;
						if (!user.Owner.EntityList.Contains(chat))
                        {
							if (!user.Owner.EntityList.Contains(Player.Instance.User))
							{
								user.Owner.LobbyCommandQueue.Enqueue(new EntityShareCommand(Player.Instance.User));
								isShared = true;
							}

							user.Owner.LobbyCommandQueue.Enqueue(new EntityShareCommand(chat));
                        }
						user.Owner.LobbyCommandQueue.Enqueue(command);

						if (isShared)
							user.Owner.LobbyCommandQueue.Enqueue(new EntityUnshareCommand(Player.Instance.User));
					}
					break;
			}
		}

		public string Message { get; set; }
	}
}
