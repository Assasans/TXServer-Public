using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Bonus;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle.Bonus
{
	[SerialVersionUID(-4179984519411113540L)]
	public class BonusTakingRequestEvent : ECSEvent
	{
		public void Execute(Player player, Entity bonus, Entity tank)
		{
			Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.BattleEntity.GetComponent<BattleGroupComponent>().Key == tank.GetComponent<BattleGroupComponent>().Key);

			CommandManager.BroadcastCommands(battle.MatchPlayers.Select(x => x.Player),
				new SendEventCommand(new BonusTakenEvent(), bonus));

			BattleBonus battleBonus = battle.BattleBonuses.Single(b => b.Bonus == bonus);
			BonusType bonusType = battleBonus.BattleBonusType;

			if (battleBonus.BonusState != BonusState.Spawned)
				return;

			if (battleBonus.BattleBonusType == BonusType.GOLD)
            {
				battleBonus.BonusState = BonusState.Unused;
			}
			else 
				battleBonus.BonusState = BonusState.Redrop;

			switch (bonusType)
            {
				case BonusType.ARMOR:
					if (tank.GetComponent<ArmorEffectComponent>() == null)
						tank.AddComponent(new ArmorEffectComponent());
					break;
				case BonusType.DAMAGE:
					if (tank.GetComponent<DamageEffectComponent>() == null)
						tank.AddComponent(new DamageEffectComponent());
					break;
				case BonusType.GOLD:
					UserMoneyComponent userMoneyComponent = player.Data.SetCrystals(player.Data.Crystals + battleBonus.GoldboxCrystals);
					player.User.ChangeComponent(userMoneyComponent);
					battle.MatchPlayers.Select(x => x.Player).SendEvent(new GoldTakenNotificationEvent(), player.BattleLobbyPlayer.BattlePlayer.BattleUser);
					break;
				case BonusType.REPAIR:
					if (tank.GetComponent<HealingEffectComponent>() == null)
						tank.AddComponent(new HealingEffectComponent());
					break;
				case BonusType.SPEED:
					if (tank.GetComponent<TurboSpeedEffectComponent>() == null)
						tank.AddComponent(new TurboSpeedEffectComponent());
					break;
            }

			battle.MatchPlayers.Select(x => x.Player).UnshareEntity(battleBonus.Bonus);
		}
	}
}