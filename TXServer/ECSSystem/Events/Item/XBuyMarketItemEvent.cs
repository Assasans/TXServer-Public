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
		public void Execute(Player player, Entity user, Entity item)
		{
			var XCrystals = player.Data.SetXCrystals(player.Data.XCrystals - Price);

			Entity newItem = new Entity(new TemplateAccessor(Activator.CreateInstance((item.TemplateAccessor.Template as IMarketItemTemplate).UserItemType) as IEntityTemplate, item.TemplateAccessor.ConfigPath),
				item.Components.ToArray());
			newItem.Components.Add(new UserGroupComponent(user));
			(newItem.TemplateAccessor.Template as IUserItemTemplate).AddUserItemComponents(player, newItem);

			CommandManager.SendCommands(player,
				new EntityShareCommand(newItem),
				new ComponentChangeCommand(user, XCrystals));

			if (newItem.TemplateAccessor.Template as IMountableItemTemplate != null)
			{
				new MountItemEvent().Execute(player, newItem);
			}
		}

		public int Price { get; set; }

		public int Amount { get; set; }
	}
}
