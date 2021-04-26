using System;
using System.Collections.Generic;
using System.Linq;

using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.EntityTemplates.Item.Slot;

namespace TXServer.Core.Battles.Module {
	public class TickHandler {
		public TickHandler(DateTimeOffset time, Action action) {
			Time = time;
			Action = action;
		}

		public DateTimeOffset Time { get; }
		public Action Action { get; }
	}
	
	public abstract class BattleModule {
		protected BattleModule(MatchPlayer matchPlayer, Entity moduleEntity) {
			MatchPlayer = matchPlayer;

			SlotEntity = SlotUserItemTemplate.CreateEntity(moduleEntity, matchPlayer.Player.BattlePlayer);
			ModuleEntity = moduleEntity;

			tickHandlers = new List<TickHandler>();
			nextTickHandlers = new List<Action>();
		}

		public MatchPlayer MatchPlayer { get; }

		public Entity SlotEntity { get; }
		public Entity ModuleEntity { get; }
		
		public bool IsEnabled { get; set; }

		public TimeSpan CooldownDuration { get; set; }
		public DateTimeOffset? CooldownStart { get; set; }
		public DateTimeOffset? CooldownEnd => CooldownStart + CooldownDuration;

		public bool IsOnCooldown => ModuleEntity.HasComponent<InventoryCooldownStateComponent>();

		private readonly List<TickHandler> tickHandlers;
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

		protected virtual void Tick() {
			IsEnabled = MatchPlayer.Battle.BattleState is BattleState.Running or BattleState.WarmUp &&
			            MatchPlayer.TankState == TankState.Active;
		}

		/// <summary>
		/// Schedules an action to run at next module tick
		/// </summary>
		/// <param name="handler">Action to run at next module tick</param>
		protected void Schedule(Action handler) {
			nextTickHandlers.Add(handler);
		}

		/// <summary>
		/// Schedules an action to run at specified time
		/// </summary>
		/// <param name="time">Time at which action should run</param>
		/// <param name="handler">Action to run at specified time</param>
		protected void Schedule(DateTimeOffset time, Action handler) {
			tickHandlers.Add(new TickHandler(time, handler));
		}

		/// <summary>
		/// Schedules an action to run after specified time
		/// </summary>
		/// <param name="timeSpan">TimeSpan after which action should run</param>
		/// <param name="handler">Action to run at specified time</param>
		protected void Schedule(TimeSpan timeSpan, Action handler) {
			Schedule(DateTimeOffset.Now + timeSpan, handler);
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

			foreach(TickHandler handler in tickHandlers.Where(handler => DateTimeOffset.Now >= handler.Time).ToArray()) {
				tickHandlers.Remove(handler);

				handler.Action();
			}
			
			foreach(Action handler in nextTickHandlers.ToArray()) {
				nextTickHandlers.Remove(handler);

				handler();
			}

			Tick();

			if(IsEnabled) {
				if(ModuleEntity.HasComponent<InventoryEnabledStateComponent>()) return;

				ModuleEntity.AddComponent(new InventoryEnabledStateComponent());
			}
			else {
				if(!ModuleEntity.HasComponent<InventoryEnabledStateComponent>()) return;

				ModuleEntity.RemoveComponent<InventoryEnabledStateComponent>();
			}
		}
	}
}
