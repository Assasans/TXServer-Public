using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
	[SerialVersionUID(1503469936379L)]
	public class ElevatedAccessUserDropSupplyEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
        {
            if (!player.Data.Admin) return;

            player.BattlePlayer?.Battle.DropSpecificBonusType(BonusType);
        }
		public BonusType BonusType { get; set; }
	}
}
