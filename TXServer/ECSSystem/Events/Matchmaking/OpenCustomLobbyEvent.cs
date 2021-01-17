using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1547630520757L)]
	public class OpenCustomLobbyEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			PlayerData data = player.Data;

			foreach (Core.Battles.Battle battle in ServerConnection.BattlePool)
            {
				if (battle.Owner == player) {
					battle.IsOpen = true;
					CommandManager.SendCommands(player, new ComponentAddCommand(battle.BattleLobbyEntity, new OpenToConnectLobbyComponent()));
					int price = 1000;  // 1000 Blue-Crystals standard price for opening custom battles
					if (player.User.GetComponent<PremiumAccountBoostComponent>() != null)
					{
						price = 0;  // official premium pass feature: open custom battles for free
					}
					UserMoneyComponent crystals = data.SetCrystals(data.Crystals - price);
					CommandManager.SendCommands(player, new ComponentChangeCommand(entity, crystals));
					break;
                }
            }
		}
	}
}