using System;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1480325268669L)]
	public class OpenContainerEvent : ECSEvent
	{
		public void Execute(Entity container)
        {
			Entity notification = new Entity(new TemplateAccessor(new NewItemNotificationTemplate(), "notification/newitem"),
				new NotificationGroupComponent(container),
				new NewItemNotificationComponent(ExtraItems.GlobalItems.Crystal, 10),
				new NotificationComponent(NotificationPriority.MESSAGE));

			UserItemCounterComponent component = container.GetComponent<UserItemCounterComponent>();

			if (Amount > component.Count) throw new ArgumentOutOfRangeException(nameof(Amount));
			component.Count -= Amount;

			CommandManager.SendCommands(Player.Instance.Socket,
				new ComponentChangeCommand(container, component),
				new EntityShareCommand(notification),
				new SendEventCommand(new ShowNotificationGroupEvent(1), container));
        }

        public long Amount { get; set; }
    }
}
