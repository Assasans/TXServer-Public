using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1473161186167L)]
	public class ExchangeCrystalsEvent : ECSEvent
	{
		public void Execute(Player player, Entity user)
		{
			player.Data.SetXCrystals(player.Data.XCrystals - XCrystals);
			player.Data.SetCrystals(player.Data.Crystals + XCrystals * 50);
		}
		
		public long XCrystals { get; set; }
	}
}
