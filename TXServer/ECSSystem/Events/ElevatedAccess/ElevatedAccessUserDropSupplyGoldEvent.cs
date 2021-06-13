using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
	[SerialVersionUID(636403856821541392L)]
	public class ElevatedAccessUserDropSupplyGoldEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
        {
            if (!player.Data.Admin) return;

            player.BattlePlayer?.Battle.DropSpecificBonusType(BonusType.GOLD, player.Data.Username);
        }
		public GoldType GoldType { get; set; }
	}
}
