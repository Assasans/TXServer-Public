using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Effect.EMP;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
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

			TickHandlers = new List<TickHandler>();
			_nextTickHandlers = new List<Action>();
		}

        public abstract void Activate();
		public virtual void Deactivate() { }
        public virtual void Init()
        {
            var ammunitionComponent = Config.GetComponent<ModuleAmmunitionPropertyComponent>(ConfigPath);
            var cooldownComponent = Config.GetComponent<ModuleCooldownPropertyComponent>(ConfigPath);
            var durationComponent = Config.GetComponent<ModuleEffectDurationPropertyComponent>(ConfigPath, false);

            CooldownDuration = cooldownComponent.UpgradeLevel2Values[Level - 1];
            Duration = durationComponent?.UpgradeLevel2Values[Level - 1] ?? 0;
            MaxAmmunition = (int) ammunitionComponent.UpgradeLevel2Values[Level - 1];
        }

        private void ActivateCooldown()
        {
            if (CooldownEnd is not null)
                AmmunitionWaitingOnCooldown++;
            else
            {
                CooldownEnd = DateTimeOffset.UtcNow.AddMilliseconds(CooldownDuration);
                ModuleEntity.AddComponent(new InventoryCooldownStateComponent((int) CooldownDuration, DateTime.UtcNow));
            }
        }
        private void CheckForCooldownEnd()
        {
            if (CooldownEnd is not null && DateTimeOffset.UtcNow >= CooldownEnd)
                DeactivateCooldown();
        }

        public void DeactivateCooldown()
        {
            CurrentAmmunition++;
            if (AmmunitionWaitingOnCooldown > 0)
            {
                CooldownEnd = DateTimeOffset.UtcNow.AddMilliseconds(CooldownDuration);
                AmmunitionWaitingOnCooldown--;
            }
            else
            {
                CooldownEnd = null;
                ModuleEntity.TryRemoveComponent<InventoryCooldownStateComponent>();
            }
        }

        public void ActivateEmpLock(float duration)
        {
            if (IsSupply || IsCheat)
            {
                Deactivate();
                return;
            }

            if (IsEmpLocked && EmpLockEnd != null)
            {
                EmpLockEnd = EmpLockEnd.Value.AddMilliseconds(duration);
                return;
            }

            ModuleEntity.AddComponent(new SlotLockedByEMPComponent());
            if (ModuleEntity.HasComponent<InventorySlotTemporaryBlockedByServerComponent>())
                ModuleEntity.ChangeComponent<InventorySlotTemporaryBlockedByServerComponent>(component =>
                {
                    component.BlockTimeMS += (long)duration;
                    component.StartBlockTime = DateTime.UtcNow;
                });
            else
                ModuleEntity.AddComponent(
                    new InventorySlotTemporaryBlockedByServerComponent((long) duration, DateTime.UtcNow));

            EmpLockEnd = DateTimeOffset.UtcNow.AddMilliseconds(duration);

            if (IsAffectedByEmp) Deactivate();
        }
        private void CheckForEmpEnd()
        {
            if (EmpLockEnd == null || IsEmpLocked) return;

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
            TickHandlers.Clear();

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

        private void CheckCheatWaitingForTank()
        {
            if (!IsWaitingForTank || MatchPlayer.TankState != TankState.Active) return;

            IsWaitingForTank = false;
            IsCheat = true;
            Activate();
        }


		protected virtual void Tick()
        {
			IsEnabled = MatchPlayer.Battle.BattleState is BattleState.Running or BattleState.WarmUp &&
			            MatchPlayer.TankState == TankState.Active;
        }

        public void ModuleTick()
        {
            CheckForCooldownEnd();
            CheckForEmpEnd();

            if (!EffectIsActive && !IsEmpLocked && AlwaysActiveExceptEmp &&
                MatchPlayer.Battle.BattleState is BattleState.WarmUp or BattleState.MatchBegins or BattleState.Running)
                Activate();

            ProcessDelayedActions();
            CheckCheatWaitingForTank();
            Tick();
        }

        private void ProcessDelayedActions()
        {
            foreach(TickHandler handler in TickHandlers.Where(handler => DateTimeOffset.UtcNow >= handler.Time).ToArray())
            {
                TickHandlers.Remove(handler);
                handler.Action();
            }

            foreach(Action handler in _nextTickHandlers.ToArray())
            {
                _nextTickHandlers.Remove(handler);
                handler();
            }
        }

        /// <summary>
        /// Schedules an action to run at next module tick
        /// </summary>
        /// <param name="handler">Action to run at next module tick</param>
        protected void Schedule(Action handler) {
            _nextTickHandlers.Add(handler);
        }

        /// <summary>
        /// Schedules an action to run at specified time
        /// </summary>
        /// <param name="time">Time at which action should run</param>
        /// <param name="handler">Action to run at specified time</param>
        private void Schedule(DateTimeOffset time, Action handler) => TickHandlers.Add(new TickHandler(time, handler));

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
        public Entity EffectEntity { get; protected set; }
        protected List<Entity> EffectEntities { get; } = new();
        public ModuleBehaviourType ModuleType { get; set; }

        public bool IsCheat { get; set; }
        public bool IsEnabled
        {
            get => _isEnabled;
            protected set
            {
                if (value == _isEnabled) return;

                _isEnabled = value;
                if (value is false)
                    ModuleEntity.TryRemoveComponent<InventoryEnabledStateComponent>();
                else if (!ModuleEntity.HasComponent<InventoryEnabledStateComponent>())
                    ModuleEntity.AddComponent(new InventoryEnabledStateComponent());
            }
        }
        private bool _isEnabled = true;
        public bool IsSupply { get; set; }
        public bool EffectIsActive => EffectEntity is not null || EffectEntities.Any();

        public bool ActivateOnTankSpawn { get; protected set; }
        protected bool AlwaysActiveExceptEmp { get; set; }
        public bool IsWaitingForTank { get; set; }
        public bool DeactivateCheat { get; set; }
        public bool DeactivateOnTankDisable { get; protected set; } = true;

        private DateTimeOffset? EmpLockEnd { get; set; }
        protected bool IsAffectedByEmp { get; set; } = true;
        protected bool IsEmpLocked => EmpLockEnd != null && EmpLockEnd > DateTimeOffset.UtcNow;

        public float CooldownDuration { get; set; }
        private DateTimeOffset? CooldownEnd { get; set; }
        public float Duration { get; set; }
        public bool IsOnCooldown => CurrentAmmunition < 1;

        public int CurrentAmmunition
        {
            get => ModuleEntity.GetComponent<InventoryAmmunitionComponent>().CurrentCount;
            set
            {
                if (CurrentAmmunition > value) ActivateCooldown();

                ModuleEntity.ChangeComponent<InventoryAmmunitionComponent>(component =>
                    component.CurrentCount = value);

                MatchPlayer.SendEvent(new InventoryAmmunitionChangedEvent(), ModuleEntity);
            }
        }
        private int MaxAmmunition
        {
            get => ModuleEntity.GetComponent<InventoryAmmunitionComponent>().MaxCount;
            set
            {
                if (ModuleEntity.HasComponent<InventoryAmmunitionComponent>())
                {
                    ModuleEntity.ChangeComponent<InventoryAmmunitionComponent>(component =>
                        component.CurrentCount = component.MaxCount = value);
                    MatchPlayer.SendEvent(new InventoryAmmunitionChangedEvent(), ModuleEntity);
                }
                else
                    ModuleEntity.AddComponent(new InventoryAmmunitionComponent(value));
            }
        }
        private int AmmunitionWaitingOnCooldown { get; set; }

        public string ConfigPath { get; set; }
        public int Level { get; set; }

        public readonly List<TickHandler> TickHandlers;
        private readonly List<Action> _nextTickHandlers;
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
