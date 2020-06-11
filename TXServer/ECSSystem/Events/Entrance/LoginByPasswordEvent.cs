using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.ECSSystem.Events;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1437480091995)]
	public class LoginByPasswordEvent : ECSEvent
	{
		public void Execute(Entity clientSession)
		{
			if (Player.Instance.Uid == null)
			{
				CommandManager.SendCommands(Player.Instance.Socket,
					new SendEventCommand(new LoginFailedEvent(), clientSession),
					new SendEventCommand(new InvalidPasswordEvent(), clientSession));
				return;
			}

			_ = clientSession ?? throw new ArgumentNullException(nameof(clientSession));

			Entity user = UserTemplate.CreateEntity(Player.Instance.Uid);
			Player.Instance.User = user;

			List<Command> collectedCommands = new List<Command>()
			{
				new EntityShareCommand(user),
				new ComponentAddCommand(clientSession, new UserGroupComponent(user.EntityId)),
			};

			collectedCommands.AddRange(from collectedEntity in ResourceManager.GetEntities(user)
									   select new EntityShareCommand(collectedEntity));

			collectedCommands.AddRange(MountItemEvent.MountPresetItems(Player.Instance.CurrentPreset));
			collectedCommands.Add(MountItemEvent.MountAvatar((Player.Instance.UserItems[typeof(Avatars)] as Avatars).Tankist));

			collectedCommands.AddRange(new Command[] {
				new SendEventCommand(new PaymentSectionLoadedEvent(), user),
				new ComponentAddCommand(user, new UserOnlineComponent()),
				new SendEventCommand(new FriendsLoadedEvent(), Player.Instance.ClientSession)
			});

			CommandManager.SendCommands(Player.Instance.Socket, collectedCommands);
		}

		public string HardwareFingerprint { get; set; }
		public string PasswordEncipher { get; set; }
		public bool RememberMe { get; set; }
	}
}
