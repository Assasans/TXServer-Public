using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Effect.EMP;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.EntityTemplates.Item.Slot;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles.Effect {
    public abstract class BattleModule
    {
		protected BattleModule(MatchPlayer matchPlayer, Entity moduleEntity)
        {
			MatchPlayer = matchPlayer;

            if (moduleEntity != null)
			    SlotEntity = SlotUserItemTemplate.CreateEntity(moduleEntity, matchPlayer.Player.BattlePlayer);
			ModuleEntity = moduleEntity;

			tickHandlers = new List<TickHandler>();
			nextTickHandlers = new List<Action>();
		}

        // TODO(Assasans): Cooldown has visual bugs on client
		public void StartCooldown()
        {
            DateTimeOffset time = DateTimeOffset.UtcNow;

			CooldownStart = time;

			/*if(!ModuleEntity.HasComponent<InventorySlotTemporaryBlockedByServerComponent>()) {
				ModuleEntity.AddComponent(
					new InventorySlotTemporaryBlockedByServerComponent((long) CooldownDuration, time.UtcDateTime)
				);
			}*/

            ModuleEntity.TryRemoveComponent<InventoryEnabledStateComponent>();
            ModuleEntity.AddComponent(new InventoryCooldownStateComponent((int) CooldownDuration, time.UtcDateTime));
            MatchPlayer.SendEvent(new BattleUserInventoryCooldownSpeedChangedEvent(), ModuleEntity);
		}

		public abstract void Activate();
		public virtual void Deactivate() { }
        public virtual void Init() {}

        public void ActivateEmpLock(float duration)
        {
            ModuleEntity.AddComponent(new SlotLockedByEMPComponent());
            if (ModuleEntity.HasComponent<InventorySlotTemporaryBlockedByServerComponent>())
                ModuleEntity.ChangeComponent<InventorySlotTemporaryBlockedByServerComponent>(component =>
                {
                    component.BlockTimeMS += (long)duration;
                    component.StartBlockTime = DateTime.Now;
                });
            else
                ModuleEntity.AddComponent(
                    new InventorySlotTemporaryBlockedByServerComponent((long) duration, DateTime.Now));

            EmpLockEnd = DateTimeOffset.Now.AddMilliseconds(duration);

            if (IsAffectedByEmp) Deactivate();
        }
        private void DeactivateEmpLock()
        {
            ModuleEntity.RemoveComponent<SlotLockedByEMPComponent>();
            ModuleEntity.RemoveComponent<InventorySlotTemporaryBlockedByServerComponent>();
            EmpLockEnd = null;
        }

        public void ShareEffect(Player joiningPlayer)
        {
            if (EffectEntity != null)
                joiningPlayer.ShareEntities(EffectEntity);
            joiningPlayer.ShareEntities(EffectEntities);
        }
        public void UnshareEffect(Player leavingPlayer)
        {
            if (leavingPlayer == MatchPlayer.Player)
            {
                Deactivate();
                if (ModuleEntity is not null)
                    leavingPlayer.UnshareEntities(ModuleEntity, SlotEntity);
            }

            if (EffectEntity != null && leavingPlayer.EntityList.Contains(EffectEntity))
                leavingPlayer.UnshareEntities(EffectEntity);
            foreach (Entity effectEntity in EffectEntities.Where(effectEntity =>
                leavingPlayer.EntityList.Contains(effectEntity)))
                leavingPlayer.UnshareEntities(effectEntity);
        }

        protected void ChangeDuration(float duration)
        {
            tickHandlers.Clear();

            if (IsCheat)
            {
                if (GetType() == typeof(RepairKitModule))
                {
                    DeactivateCheat = true;
                    Deactivate();
                    DeactivateCheat = false;
                    IsCheat = true;
                    Activate();
                    return;
                }
                if (GetType() == typeof(TurboSpeedModule) && IsCheat)
                    MatchPlayer.Tank.ChangeComponent<SpeedComponent>(component => component.Speed = float.MaxValue);
            }

            EffectEntity.ChangeComponent<DurationConfigComponent>(component => component.Duration = (long)duration);
            EffectEntity.RemoveComponent<DurationComponent>();
            EffectEntity.AddComponent(new DurationComponent(DateTime.UtcNow));

            Schedule(TimeSpan.FromMilliseconds(duration), Deactivate);
        }

		protected virtual void Tick()
        {
			IsEnabled = MatchPlayer.Battle.BattleState is BattleState.Running or BattleState.WarmUp &&
			            MatchPlayer.TankState == TankState.Active;
        }

        public void ModuleTick()
        {
            if (CooldownStart != null && DateTimeOffset.UtcNow >= CooldownEnd)
            {
                ModuleEntity.TryRemoveComponent<InventoryCooldownStateComponent>();

                if (ModuleType is not ModuleBehaviourType.PASSIVE)
                    ModuleEntity.AddComponent(new InventoryEnabledStateComponent());

                CooldownStart = null;
			}

            if (EmpLockEnd != null && !IsEmpLocked) DeactivateEmpLock();

            if (!EffectIsActive && !IsEmpLocked && AlwaysActiveExceptEmp &&
                MatchPlayer.Battle.BattleState is BattleState.WarmUp or BattleState.MatchBegins or BattleState.Running)
                Activate();

            foreach(TickHandler handler in tickHandlers.Where(handler => DateTimeOffset.UtcNow >= handler.Time).ToArray())
            {
				tickHandlers.Remove(handler);
                handler.Action();
			}

			foreach(Action handler in nextTickHandlers.ToArray())
            {
				nextTickHandlers.Remove(handler);
                handler();
			}

			Tick();

            if (ModuleEntity is not null && ModuleType is not ModuleBehaviourType.PASSIVE)
            {
                if (IsEnabled && !IsOnCooldown)
                {
                    if (ModuleEntity.HasComponent<InventoryEnabledStateComponent>()) return;
                    ModuleEntity.AddComponent(new InventoryEnabledStateComponent());
                }
                else
                    ModuleEntity.TryRemoveComponent<InventoryEnabledStateComponent>();
            }

            if (CheatWaitingForTank && MatchPlayer.TankState == TankState.Active)
            {
                CheatWaitingForTank = false;
                IsCheat = true;
                Activate();
            }
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
        protected void Schedule(DateTimeOffset time, Action handler) => tickHandlers.Add(new TickHandler(time, handler));

        /// <summary>
        /// Schedules an action to run after specified time
        /// </summary>
        /// <param name="timeSpan">TimeSpan after which action should run</param>
        /// <param name="handler">Action to run at specified time</param>
        protected void Schedule(TimeSpan timeSpan, Action handler) => Schedule(DateTimeOffset.UtcNow + timeSpan, handler);

        protected MatchPlayer MatchPlayer { get; }

        public Entity MarketItem { get; set; }
        public Entity SlotEntity { get; }
        public Entity ModuleEntity { get; }
        public Entity EffectEntity { get; set; }
        public List<Entity> EffectEntities { get; set; } = new();
        public ModuleBehaviourType ModuleType { get; set; }

        public bool IsCheat { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSupply { get; set; }
        public bool EffectIsActive => EffectEntity is not null || EffectEntities.Any();
        protected bool IsEmpLocked => EmpLockEnd != null && EmpLockEnd > DateTimeOffset.Now;

        public bool ActivateOnTankSpawn { get; set; }
        public bool AlwaysActiveExceptEmp { get; set; }
        public bool CheatWaitingForTank { get; set; }
        public bool DeactivateCheat { get; set; }
        public bool DeactivateOnTankDisable { get; set; } = true;
        protected bool IsAffectedByEmp { get; set; } = true;


        public float Duration { get; set; }

        private DateTimeOffset? EmpLockEnd { get; set; }

        public float CooldownDuration { get; set; }
        public DateTimeOffset? CooldownStart { get; set; }
        public DateTimeOffset? CooldownEnd => CooldownStart?.AddMilliseconds(CooldownDuration);

        public bool IsOnCooldown =>
            ModuleEntity is not null && ModuleEntity.HasComponent<InventoryCooldownStateComponent>();

        public string ConfigPath { get; set; }
        public int Level { get; set; }

        public readonly List<TickHandler> tickHandlers;
        private readonly List<Action> nextTickHandlers;
	}

    public class TickHandler {
        public TickHandler(DateTimeOffset time, Action action) {
            Time = time;
            Action = action;
        }

        public DateTimeOffset Time { get; }
        public Action Action { get; }
    }
}
