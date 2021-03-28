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
	[SerialVersionUID(-4179984519411113540L)]
	public class BonusTakingRequestEvent : ECSEvent
	{
		public void Execute(Player player, Entity bonus, Entity tank)
		{
			Core.Battles.Battle battle = player.BattlePlayer.Battle;
			BattleBonus battleBonus = battle.BattleBonuses.Single(b => b.Bonus == bonus);
			BonusType bonusType = battleBonus.BattleBonusType;
			SupplyEffect supplyEffect = null;

			if (battleBonus.BonusState != BonusState.Spawned)
				return;
			if (battleBonus.BattleBonusType == BonusType.GOLD)
				battleBonus.BonusState = BonusState.Unused;
			else
				battleBonus.BonusState = BonusState.Redrop;

			battle.MatchPlayers.Select(x => x.Player).SendEvent(new BonusTakenEvent(), bonus);

			switch (bonusType)
			{
				case BonusType.GOLD:
					UserMoneyComponent userMoneyComponent = player.Data.SetCrystals(player.Data.Crystals + battleBonus.GoldboxCrystals);
					player.User.ChangeComponent(userMoneyComponent);
					battle.MatchPlayers.Select(x => x.Player).SendEvent(new GoldTakenNotificationEvent(), player.BattlePlayer.MatchPlayer.BattleUser);
					break;
				default:
					supplyEffect = new SupplyEffect(bonusType, player.BattlePlayer.MatchPlayer);
					break;
			}

			player.BattlePlayer.MatchPlayer.UserResult.BonusesTaken += 1;
			battle.MatchPlayers.Select(x => x.Player).UnshareEntity(battleBonus.Bonus);
		}
	}
}