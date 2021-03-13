using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle.Bonus
{
	[SerialVersionUID(636403856821541392L)]
	public class ElevatedAccessUserDropSupplyGoldEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.MatchPlayers.Contains(player.BattleLobbyPlayer));

			List<int> goldboxesIndex = new List<int>();
			int index = 0;
			foreach (BattleBonus battleBonus in battle.BattleBonuses)
            {
				if (battleBonus.BattleBonusType == BonusType.GOLD) 
					if (battleBonus.BonusState == BonusState.Unused)
				        goldboxesIndex.Add(index);
				++index;
            }

			if (!goldboxesIndex.Any() || player.User.GetComponent<UserAdminComponent>() == null)
				return;

			int goldboxIndex = goldboxesIndex[new Random().Next(goldboxesIndex.Count)];
			battle.BattleBonuses[goldboxIndex].BonusState = BonusState.New;

			battle.AllBattlePlayers.Select(x => x.Player).SendEvent(new GoldScheduleNotificationEvent(player.User.GetComponent<UserUidComponent>().Uid), entity);
		}
		public GoldType GoldType { get; set; }
	}
}