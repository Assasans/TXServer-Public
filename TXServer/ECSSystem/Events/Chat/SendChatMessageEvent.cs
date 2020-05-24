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
		public void Execute(Entity userOrChannel)
        {
			if (userOrChannel.TemplateAccessor.Template is GeneralChatTemplate)
			{
				foreach (Player player in ServerLauncher.Pool)
				{
					player.LobbyCommandQueue.Enqueue(new SendEventCommand(new ChatMessageReceivedEvent
					{
						Message = this.Message,
						SystemMessage = false,
						UserId = Player.Instance.User.EntityId,
						UserUid = Player.Instance.User.GetComponent<UserUidComponent>().Uid,
						UserAvatarId = Player.Instance.User.GetComponent<UserAvatarComponent>().Id
					}, userOrChannel));
				}
			}
		}

		public string Message { get; set; }
	}
}
