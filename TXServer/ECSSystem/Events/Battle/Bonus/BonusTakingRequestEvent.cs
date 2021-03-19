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
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle.Bonus
{
	[SerialVersionUID(-4179984519411113540L)]
	public class BonusTakingRequestEvent : ECSEvent
	{
		public void Execute(Player player, Entity bonus, Entity tank)
		{
			Core.Battles.Battle battle = player.BattlePlayer.Battle;

			CommandManager.BroadcastCommands(battle.MatchPlayers.Select(x => x.Player),
				new SendEventCommand(new BonusTakenEvent(), bonus));

			BattleBonus battleBonus = battle.BattleBonuses.Single(b => b.Bonus == bonus);
			BonusType bonusType = battleBonus.BattleBonusType;

			if (battleBonus.BonusState != BonusState.Spawned)
				return;

			if (battleBonus.BattleBonusType == BonusType.GOLD)
				battleBonus.BonusState = BonusState.Unused;
			else 
				battleBonus.BonusState = BonusState.Redrop;

			if (!player.BattlePlayer.MatchPlayer.SupplyEffects.ContainsKey(bonusType))
            {
				switch (bonusType)
				{
					case BonusType.ARMOR:
						tank.AddComponent(new ArmorEffectComponent());
						break;
					case BonusType.DAMAGE:
						tank.AddComponent(new DamageEffectComponent());
						break;
					case BonusType.GOLD:
						UserMoneyComponent userMoneyComponent = player.Data.SetCrystals(player.Data.Crystals + battleBonus.GoldboxCrystals);
						player.User.ChangeComponent(userMoneyComponent);
						battle.MatchPlayers.Select(x => x.Player).SendEvent(new GoldTakenNotificationEvent(), player.BattlePlayer.MatchPlayer.BattleUser);
						break;
					case BonusType.REPAIR:
						player.BattlePlayer.MatchPlayer.Tank.ChangeComponent(new TemperatureComponent(0));

						HealthComponent healthComponent = player.BattlePlayer.MatchPlayer.Tank.GetComponent<HealthComponent>();
						healthComponent.CurrentHealth = healthComponent.MaxHealth;
						player.BattlePlayer.MatchPlayer.Tank.ChangeComponent(healthComponent);
						player.SendEvent(new HealthChangedEvent(), player.BattlePlayer.MatchPlayer.Tank);

						// todo: research healing effect animation trigger
						if (tank.GetComponent<HealingEffectComponent>() == null)
						    tank.AddComponent(new HealingEffectComponent());
						break;
					case BonusType.SPEED:
						tank.AddComponent(new TurboSpeedEffectComponent());
						tank.ChangeComponent(new SpeedComponent(float.MaxValue, 98f, 13));
						break;
				}
			}

			if (bonusType != BonusType.REPAIR && bonusType != BonusType.GOLD)
			{
				player.BattlePlayer.MatchPlayer.SupplyEffects.Remove(bonusType);
				player.BattlePlayer.MatchPlayer.SupplyEffects.Add(bonusType, 30);
			}

			player.BattlePlayer.MatchPlayer.UserResult.BonusesTaken += 1;
			battle.MatchPlayers.Select(x => x.Player).UnshareEntity(battleBonus.Bonus);
		}
	}
}