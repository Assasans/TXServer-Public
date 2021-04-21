using System;
using System.Threading;
using System.Threading.Tasks;

using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle {
	[SerialVersionUID(1486015564167L)]
	public class ActivateModuleEvent : ECSEvent {
		public void Execute(Player player, Entity module) {
			// Should check module type instead of slot index
			SlotUserItemInfoComponent slotItem = module.GetComponent<SlotUserItemInfoComponent>();
			if(slotItem.Slot == 0) {
				const int cooldown = 5000;

				InventorySlotTemporaryBlockedByServerComponent blocked =
					module.GetComponent<InventorySlotTemporaryBlockedByServerComponent>();
				if(blocked == null) {
					_ = new SupplyEffect(BonusType.SPEED, player.BattlePlayer.MatchPlayer, cheat: false);

					DateTime startDate = DateTime.Now;

					module.AddComponent(new InventorySlotTemporaryBlockedByServerComponent(cooldown, startDate));
					module.AddComponent(new InventoryCooldownStateComponent(cooldown, startDate));

					// TODO(Assasans): Move to battle thread
					Task.Run(
						async () => {
							await Task.Delay(cooldown);

							// if(DateTimeOffset.Now.ToUnixTimeMilliseconds() > blocked.StartBlockTime.Time + blocked.BlockTimeMS) {
							if(module.HasComponent<InventorySlotTemporaryBlockedByServerComponent>()) {
								module.RemoveComponent<InventorySlotTemporaryBlockedByServerComponent>();
							}

							if(module.HasComponent<InventoryCooldownStateComponent>()) {
								module.RemoveComponent<InventoryCooldownStateComponent>();
							}
							// }
						}
					);
				}
			}
			else if(slotItem.Slot == Slot.SLOT2) {
				const int cooldown = 1000;

				InventorySlotTemporaryBlockedByServerComponent blocked =
					module.GetComponent<InventorySlotTemporaryBlockedByServerComponent>();
				if(blocked == null) {
					player.BattlePlayer.Battle.DropSpecificBonusType(BonusType.GOLD, player.UniqueId);

					DateTime startDate = DateTime.Now;

					module.AddComponent(new InventorySlotTemporaryBlockedByServerComponent(cooldown, startDate));
					module.AddComponent(new InventoryCooldownStateComponent(cooldown, startDate));

					// TODO(Assasans): Move to battle thread
					Task.Run(
						async () => {
							await Task.Delay(cooldown);

							// if(DateTimeOffset.Now.ToUnixTimeMilliseconds() > blocked.StartBlockTime.Time + blocked.BlockTimeMS) {
							if(module.HasComponent<InventorySlotTemporaryBlockedByServerComponent>()) {
								module.RemoveComponent<InventorySlotTemporaryBlockedByServerComponent>();
							}

							if(module.HasComponent<InventoryCooldownStateComponent>()) {
								module.RemoveComponent<InventoryCooldownStateComponent>();
							}
							// }
						}
					);
				}
			}
		}

		public int ClientTime { get; set; }
	}
}
