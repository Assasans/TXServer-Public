using System;
using System.Linq;
using TXServer.Core;
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
			Entity newItem =
                new(new TemplateAccessor(
                        (IEntityTemplate) Activator.CreateInstance(
                            ((IMarketItemTemplate) item.TemplateAccessor.Template).UserItemType),
                        item.TemplateAccessor.ConfigPath),
				item.Components.ToArray());
			newItem.Components.Add(new UserGroupComponent(user));
			((IUserItemTemplate)newItem.TemplateAccessor.Template).AddUserItemComponents(player, newItem);

			player.ShareEntities(newItem);
			player.Data.SetXCrystals(player.Data.XCrystals - Price);

			if (newItem.TemplateAccessor.Template is IMountableItemTemplate)
				new MountItemEvent().Execute(player, newItem);
		}

		public int Price { get; set; }

		public int Amount { get; set; }
	}
}
