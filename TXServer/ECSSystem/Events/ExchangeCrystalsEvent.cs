using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1473161186167L)]
	public class ExchangeCrystalsEvent : ECSEvent
	{
		public long XCrystals { get; set; }

		public void Execute(Entity entity)
		{
			Component tmpComponent;
			entity.Components.TryGetValue(new UserXCrystalsComponent(0), out tmpComponent);
			UserXCrystalsComponent XCrystals = tmpComponent as UserXCrystalsComponent;

			entity.Components.TryGetValue(new UserMoneyComponent(0), out tmpComponent);
			UserMoneyComponent Crystals = tmpComponent as UserMoneyComponent;

			XCrystals.Money -= this.XCrystals;
			Crystals.Money += this.XCrystals * 50;

			CommandManager.SendCommands(Player.Instance.Socket,
				new ComponentChangeCommand(entity, XCrystals),
				new ComponentChangeCommand(entity, Crystals));
		}
	}
}
