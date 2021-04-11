using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
	[SerialVersionUID(636403856821541392L)]
	public class ElevatedAccessUserDropSupplyGoldEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			if (player.BattlePlayer != null) player.BattlePlayer.Battle.DropSpecificBonusType(BonusType.GOLD, player.User.GetComponent<UserUidComponent>().Uid);
		}
		public GoldType GoldType { get; set; }
	}
}