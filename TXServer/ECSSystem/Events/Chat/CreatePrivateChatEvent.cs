using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(636469080057216111L)]
	public class CreatePrivateChatEvent : ECSEvent
	{
		public void Execute(Player player, Entity user, Entity sourceChat)
        {
			Entity goalUser = null;
			foreach (Player p in player.Server.Connection.Pool)
            {
				if (p.UniqueId == UserUid.Replace("Survivor ", "").Replace("Deserter ", ""))
				{
					goalUser = p.User;
					break;
				}
            }
			if (goalUser is null) return;

			Entity chat = null;
			List<Entity> chatParticipants = new() { player.User, goalUser };
			foreach (Entity participant in chatParticipants)
            {
				Entity otherUser = chatParticipants.Single(user => user.EntityId != participant.EntityId);
				foreach (Entity personalChat in participant.GetComponent<PersonalChatOwnerComponent>().ChatsIs)
				{
					if (personalChat.GetComponent<ChatParticipantsComponent>().Users.Contains(otherUser))
					{
						chat = personalChat;
						break;
					}
				}
			}

			if (!player.EntityList.Contains(goalUser))
				player.ShareEntities(goalUser);
			if (chat == null)
            {
				chat = new Entity(new TemplateAccessor(new PersonalChatTemplate(), "chat"),
			    new ChatComponent(),
			    new ChatParticipantsComponent(user, goalUser));

				PersonalChatOwnerComponent component = player.User.GetComponent<PersonalChatOwnerComponent>();
				component.ChatsIs.Add(chat);
				player.User.ChangeComponent(component);
			}

			if (!player.EntityList.Contains(chat))
				player.ShareEntities(chat);
			player.UnshareEntities(goalUser);
        }

		public string UserUid { get; set; }
	}
}
