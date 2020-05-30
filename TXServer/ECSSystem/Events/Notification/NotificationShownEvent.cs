using System;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1454667308567L)]
	public class NotificationShownEvent : ECSEvent
	{
		public void Execute(Entity notification)
		{
			CommandManager.SendCommands(Player.Instance.Socket, new EntityUnshareCommand(notification));
		}
	}
}
