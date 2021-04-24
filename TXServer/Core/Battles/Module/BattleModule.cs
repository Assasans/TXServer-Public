using System;

using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.EntityTemplates.Item.Slot;

namespace TXServer.Core.Battles.Module {
	public abstract class BattleModule {
		protected BattleModule(MatchPlayer player, Entity moduleEntity) {
			Player = player;

			SlotEntity = SlotUserItemTemplate.CreateEntity(moduleEntity, player.Player.BattlePlayer);
			ModuleEntity = moduleEntity;
		}

		public MatchPlayer Player { get; }

		public Entity SlotEntity { get; }
		public Entity ModuleEntity { get; }

		public TimeSpan CooldownDuration { get; set; }
		public DateTimeOffset? CooldownStart { get; set; }
		public DateTimeOffset? CooldownEnd => CooldownStart + CooldownDuration;

		public bool IsOnCooldown => ModuleEntity.HasComponent<InventoryCooldownStateComponent>();

		// TODO(Assasans): Cooldown has visual bugs on client
		public void StartCooldown() {
			DateTimeOffset time = DateTimeOffset.Now;

			CooldownStart = time;

			if(!ModuleEntity.HasComponent<InventorySlotTemporaryBlockedByServerComponent>()) {
				ModuleEntity.AddComponent(
					new InventorySlotTemporaryBlockedByServerComponent((int)CooldownDuration.TotalMilliseconds, time.UtcDateTime)
				);
			}

			if(!ModuleEntity.HasComponent<InventoryCooldownStateComponent>()) {
				ModuleEntity.AddComponent(
					new InventoryCooldownStateComponent((int)CooldownDuration.TotalMilliseconds, time.UtcDateTime)
				);
			}
		}

		public abstract void Activate();
		public virtual void Deactivate() { }
		public virtual void Tick() { }

		public void CooldownTick() {
			if(CooldownEnd == null) return;

			if(DateTimeOffset.Now < CooldownEnd) return;

			if(ModuleEntity.HasComponent<InventoryCooldownStateComponent>()) {
				ModuleEntity.RemoveComponent<InventoryCooldownStateComponent>();
			}

			if(ModuleEntity.HasComponent<InventorySlotTemporaryBlockedByServerComponent>()) {
				ModuleEntity.RemoveComponent<InventorySlotTemporaryBlockedByServerComponent>();
			}

			CooldownStart = null;
		}
	}
}
