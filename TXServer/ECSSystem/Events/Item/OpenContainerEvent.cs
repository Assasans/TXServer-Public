using System;
using TXServer.Core;
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
		public void Execute(Player player, Entity container)
        {
			Entity notification = new(new TemplateAccessor(new NewItemNotificationTemplate(), "notification/newitem"),
				new NotificationGroupComponent(container),
				new NewItemNotificationComponent(ExtraItems.GlobalItems.Crystal, 10),
				new NotificationComponent(NotificationPriority.MESSAGE));

			container.ChangeComponent<UserItemCounterComponent>(component =>
			{
				if (Amount > component.Count) throw new ArgumentOutOfRangeException(nameof(Amount));
				component.Count -= Amount;
			});

			player.ShareEntities(notification);
			player.SendEvent(new ShowNotificationGroupEvent(1), container);
        }

        public long Amount { get; set; }
    }
}
