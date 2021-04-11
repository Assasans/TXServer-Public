using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
    [SerialVersionUID(1515481976775L)]
	public class ElevatedAccessUserWipeUserItemsEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			if (player.IsInMatch)
			{
				foreach (SupplyEffect supplyEffect in player.BattlePlayer.MatchPlayer.SupplyEffects.ToArray())
					supplyEffect.Remove();
			}
		}
	}
}
