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
		public void Execute(Player player, Entity userOrChannel)
        {
			if (userOrChannel.TemplateAccessor.Template is GeneralChatTemplate)
			{
				foreach (Player connectedPlayer in player.Server.Connection.Pool)
				{
					CommandManager.SendCommands(connectedPlayer, new SendEventCommand(new ChatMessageReceivedEvent
					{
						Message = Message,
						SystemMessage = false,
						UserId = player.User.EntityId,
						UserUid = player.User.GetComponent<UserUidComponent>().Uid,
						UserAvatarId = player.User.GetComponent<UserAvatarComponent>().Id
					}, userOrChannel));
				}
			}
		}

		public string Message { get; set; }
	}
}
