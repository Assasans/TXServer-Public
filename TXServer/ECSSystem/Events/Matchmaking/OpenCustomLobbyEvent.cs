using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1547630520757L)]
	public class OpenCustomLobbyEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			Core.Battles.Battle battle = player.BattlePlayer.Battle;

			int price = 1000;  // 1000 Blue-Crystals standard price for opening custom battles
			if (player.User.GetComponent<PremiumAccountBoostComponent>() != null)
			{
				price = 0;  // official premium pass feature: open custom battles for free
			}
			UserMoneyComponent userMoneyComponent = player.Data.SetCrystals(player.Data.Crystals - price);
			player.User.ChangeComponent(userMoneyComponent);

			((CustomBattleHandler)battle.TypeHandler).OpenBattle();
		}
	}
}