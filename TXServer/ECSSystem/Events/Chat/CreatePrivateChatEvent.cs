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

		public void Execute(Entity user, Entity sourceChat)
        {
			Entity goalUser = null;
			foreach (Player player in ServerLauncher.Pool)
            {
				if (player.Uid == UserUid)
				{
					goalUser = player.User;
					break;
				}
            }
			if (goalUser is null) return;

			Entity newChat = PersonalChatTemplate.CreateEntity(user, goalUser);

			PersonalChatOwnerComponent component = Player.Instance.User.GetComponent<PersonalChatOwnerComponent>();
			component.ChatsIs.Add(newChat);

			CommandManager.SendCommands(Player.Instance.Socket,
				new EntityShareCommand(goalUser),
				new ComponentChangeCommand(Player.Instance.User, component),
				new EntityShareCommand(newChat),
				new EntityUnshareCommand(goalUser));
        }

		public string UserUid { get; set; }
	}
}
