using System;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(636469080057216111L)]
	public class CreatePrivateChatEvent : ECSEvent
	{
		public void Execute(Entity user)
		{
			throw new NotImplementedException();
		}

		public void Execute(Player player, Entity user, Entity sourceChat)
        {
			Entity goalUser = null;
			foreach (Player p in player.Server.Connection.Pool)
            {
				if (p.GetUniqueId() == UserUid)
				{
					goalUser = p.User;
					break;
				}
            }
			if (goalUser is null) return;

			Entity newChat = new Entity(new TemplateAccessor(new PersonalChatTemplate(), "chat"),
				new ChatComponent(),
				new ChatParticipantsComponent(user, goalUser));

			PersonalChatOwnerComponent component = player.User.GetComponent<PersonalChatOwnerComponent>();
			component.ChatsIs.Add(newChat);

			CommandManager.SendCommands(player,
				new EntityShareCommand(goalUser),
				new ComponentChangeCommand(player.User, component),
				new EntityShareCommand(newChat),
				new EntityUnshareCommand(goalUser));
        }

		public string UserUid { get; set; }
	}
}
