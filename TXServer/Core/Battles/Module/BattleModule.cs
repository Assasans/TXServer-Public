using System;
using System.Collections.Generic;

using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.EntityTemplates.Item.Slot;

namespace TXServer.Core.Battles.Module {
	public abstract class BattleModule {
		protected BattleModule(MatchPlayer player, Entity moduleEntity) {
			Player = player;

			SlotEntity = SlotUserItemTemplate.CreateEntity(moduleEntity, player.Player.BattlePlayer);
			ModuleEntity = moduleEntity;

			nextTickHandlers = new List<Action>();
		}

		public MatchPlayer Player { get; }

		public Entity SlotEntity { get; }
		public Entity ModuleEntity { get; }

		public TimeSpan CooldownDuration { get; set; }
		public DateTimeOffset? CooldownStart { get; set; }
		public DateTimeOffset? CooldownEnd => CooldownStart + CooldownDuration;

		public bool IsOnCooldown => ModuleEntity.HasComponent<InventoryCooldownStateComponent>();

		private readonly List<Action> nextTickHandlers;

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

		protected virtual void Tick() { }

		/// <summary>
		/// Schedules an action to run at next module tick
		/// </summary>
		/// <param name="handler">Action to run at next module tick</param>
		protected void Schedule(Action handler) {
			nextTickHandlers.Add(handler);
		}

		public void ModuleTick() {
			if(CooldownStart != null && DateTimeOffset.Now >= CooldownEnd) {
				if(ModuleEntity.HasComponent<InventoryCooldownStateComponent>()) {
					ModuleEntity.RemoveComponent<InventoryCooldownStateComponent>();
				}

				if(ModuleEntity.HasComponent<InventorySlotTemporaryBlockedByServerComponent>()) {
					ModuleEntity.RemoveComponent<InventorySlotTemporaryBlockedByServerComponent>();
				}

				CooldownStart = null;
			}
			
			foreach(Action handler in nextTickHandlers.ToArray()) {
				nextTickHandlers.Remove(handler);

				handler();
			}

			Tick();
		}
	}
}
