using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle.Bonus
{
	[SerialVersionUID(1503469936379L)]
	public class ElevatedAccessUserDropSupplyEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.MatchPlayers.Contains(player.BattleLobbyPlayer));
			Random random = new Random();

			List<int> suppliesIndex = new List<int>();
			int index = 0;
			foreach (BattleBonus battleBonus in battle.BattleBonuses)
			{
				if (battleBonus.BattleBonusType == BonusType)
                {
					if (battleBonus.BonusState != BonusState.Spawned)
						suppliesIndex.Add(index);
				}
				++index;
			}

			if (!suppliesIndex.Any() || player.User.GetComponent<UserAdminComponent>() == null)
				return;

			int supplyIndex = suppliesIndex[random.Next(suppliesIndex.Count)];
			if (battle.BattleBonuses[supplyIndex].BattleBonusType != BonusType.GOLD)
            {
				battle.BattleBonuses[supplyIndex].BonusStateChangeCountdown = 0;
			}
			else
            {
				battle.BattleBonuses[supplyIndex].BonusState = BonusState.New;
				CommandManager.BroadcastCommands(battle.MatchPlayers.Select(x => x.Player),
					new SendEventCommand(new GoldScheduleNotificationEvent(""), entity));
			}
		}
		public BonusType BonusType { get; set; }
	}
}