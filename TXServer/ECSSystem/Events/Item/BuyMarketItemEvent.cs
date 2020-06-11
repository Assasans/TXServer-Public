using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1458203345903L)]
	public class BuyMarketItemEvent : ECSEvent
	{
		public void Execute(Entity user, Entity item)
		{
			UserMoneyComponent Crystals = user.GetComponent<UserMoneyComponent>();
			Crystals.Money -= Price;

			Entity newItem = (item.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(item, user);

			switch (newItem.TemplateAccessor.Template)
			{
				case IMountableItemTemplate _:
					CommandManager.SendCommands(Player.Instance.Socket,
						new EntityShareCommand(newItem),
						new ComponentChangeCommand(user, Crystals));
					new MountItemEvent().Execute(newItem);
					break;
				case ICountableItemTemplate _:
					UserItemCounterComponent component = newItem.GetComponent<UserItemCounterComponent>();
					component.Count += Amount;
					CommandManager.SendCommands(Player.Instance.Socket,
						new ComponentChangeCommand(newItem, component),
						new SendEventCommand(new ItemsCountChangedEvent(Amount), newItem),
						new ComponentChangeCommand(user, Crystals));
					break;
			}
		}

		public int Price { get; set; }

		public int Amount { get; set; }
	}
}
