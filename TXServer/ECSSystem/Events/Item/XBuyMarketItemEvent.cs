using System;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1473424321578L)]
	public class XBuyMarketItemEvent : ECSEvent
	{
		public void Execute(Entity user, Entity item)
		{
			UserXCrystalsComponent XCrystals = user.GetComponent<UserXCrystalsComponent>();
			XCrystals.Money -= Price;

			Entity newItem = (item.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(item, user);

			CommandManager.SendCommands(Player.Instance.Socket,
				new EntityShareCommand(newItem),
				new ComponentChangeCommand(user, XCrystals));

			if (newItem.TemplateAccessor.Template as IMountableItemTemplate != null)
			{
				new MountItemEvent().Execute(newItem);
			}
		}

		public int Price { get; set; }

		public int Amount { get; set; }
	}
}
